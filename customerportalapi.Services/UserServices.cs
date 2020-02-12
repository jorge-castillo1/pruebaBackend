using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using customerportalapi.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities.enums;
using Microsoft.Extensions.Configuration;
using customerportalapi.Services.Exceptions;
using System.Net;
using System.Threading;
using PasswordGenerator;

namespace customerportalapi.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IIdentityRepository _identityRepository;
        private readonly IConfiguration _config;

        public UserServices(IUserRepository userRepository, IProfileRepository profileRepository, IMailRepository mailRepository, IEmailTemplateRepository emailTemplateRepository, IIdentityRepository identityRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _mailRepository = mailRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _identityRepository = identityRepository;
            _config = config;
        }


        public async Task<Profile> GetProfileAsync(string dni)
        {
            //Add customer portal Business Logic
            User user = _userRepository.GetCurrentUserByDni(dni);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //1. If emailverified is false throw error
            if (!user.Emailverified)
                throw new ServiceException("User is deactivated,", HttpStatusCode.NotFound, "User", "Deactivated");

            //2. If exist complete data from external repository
            //Invoke repository
            var entity = await _profileRepository.GetProfileAsync(dni);

            //3. Set Email Principal according to external data. No two principal emails allowed
            entity.EmailAddress1Principal = false;
            entity.EmailAddress2Principal = false;

            if (entity.EmailAddress1 == user.Email)
                entity.EmailAddress1Principal = true;
            else if (entity.EmailAddress2 == user.Email)
                entity.EmailAddress2Principal = true;

            //4. Set Phone Principal according to external data. No two principal phones allowed
            entity.MobilePhone1Principal = false;
            entity.MobilePhonePrincipal = false;

            if (entity.MobilePhone1 == user.Phone && !string.IsNullOrEmpty(user.Phone))
                entity.MobilePhone1Principal = true;
            else if (entity.MobilePhone == user.Phone && !string.IsNullOrEmpty(user.Phone))
                entity.MobilePhonePrincipal = true;

            entity.Language = user.Language;
            entity.Avatar = user.Profilepicture;

            return entity;
        }

        public async Task<Profile> UpdateProfileAsync(Profile profile)
        {
            //Add customer portal Business Logic
            User user = _userRepository.GetCurrentUserByDni(profile.DocumentNumber);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //1. If emailverified is false throw error
            if (!user.Emailverified)
                throw new ServiceException("User is deactivated,", HttpStatusCode.NotFound, "User", "Deactivated");

            //2. Set Email Principal according to external data
            if (string.IsNullOrEmpty(profile.EmailAddress1) && string.IsNullOrEmpty(profile.EmailAddress2))
                throw new ServiceException("Email field can not be null.", HttpStatusCode.BadRequest, "Email", "Empty fields");

            if (profile.EmailAddress1Principal && string.IsNullOrEmpty(profile.EmailAddress1))
                throw new ServiceException("Principal email can not be null.", HttpStatusCode.BadRequest, "Principal email", "Empty field");

            if (profile.EmailAddress2Principal && string.IsNullOrEmpty(profile.EmailAddress2))
                throw new ServiceException("Principal email can not be null.", HttpStatusCode.BadRequest, "Principal email", "Empty field");

            var emailToUpdate = profile.EmailAddress1Principal ? profile.EmailAddress1 : profile.EmailAddress2;

            //3. Set Phone Principal according to data
            string phoneToUpdate = string.Empty;
            if (profile.MobilePhone1Principal && !string.IsNullOrEmpty(profile.MobilePhone1))
                phoneToUpdate = profile.MobilePhone1;
            else if (profile.MobilePhonePrincipal && !string.IsNullOrEmpty(profile.MobilePhone))
                phoneToUpdate = profile.MobilePhone;

            //4. Compare language, email and image for backend changes
            if (user.Language != profile.Language ||
                user.Profilepicture != profile.Avatar ||
                user.Email != emailToUpdate ||
                user.Phone != phoneToUpdate)
            {
                user.Language = profile.Language;
                user.Email = emailToUpdate;
                user.Phone = phoneToUpdate;
                user.Profilepicture = profile.Avatar;

                user = _userRepository.Update(user);
            }

            //5. Invoke repository for other changes
            var entity = await _profileRepository.UpdateProfileAsync(profile);
            entity.Language = user.Language;
            entity.Avatar = user.Profilepicture;
            if (entity.EmailAddress1 == user.Email)
                entity.EmailAddress1Principal = true;
            else
                entity.EmailAddress2Principal = true;

            if (entity.MobilePhone1 == user.Phone && !string.IsNullOrEmpty(user.Phone))
                entity.MobilePhone1Principal = true;
            else if (entity.MobilePhone == user.Phone && !string.IsNullOrEmpty(user.Phone))
                entity.MobilePhonePrincipal = true;

            return entity;
        }

        public async Task<bool> InviteUserAsync(Invitation value)
        {
            bool result = false;

            //1. Validate email not empty
            if (string.IsNullOrEmpty(value.Email))
                throw new ServiceException("User must have a valid email address.", HttpStatusCode.BadRequest, "Email", "Empty field");

            //2. Validate dni not empty
            if (string.IsNullOrEmpty(value.Dni))
                throw new ServiceException("User must have a valid document number.", HttpStatusCode.BadRequest, "Dni", "Empty field");

            //3. If no user exists create user
            var userType = InvitationUtils.GetUserType(value.CustomerType);
            var userName = userType == 0 ? value.Dni : "B" + value.Dni;

            var pwd = new Password(true, true, true, false, 6);
            var password = pwd.Next();

            User user = _userRepository.GetCurrentUser(userName);
            if (user.Id == null)
            {
                //4. Create user in portal database
                user = new User
                {
                    Username = userName,
                    Dni = value.Dni,
                    Email = value.Email,
                    Name = value.Fullname,
                    Password = password,
                    Language = InvitationUtils.GetLanguage(value.Language),
                    Usertype = InvitationUtils.GetUserType(value.CustomerType),
                    Emailverified = false,
                    Invitationtoken = Guid.NewGuid().ToString()
                };

                result = await _userRepository.Create(user);
            }
            else
            {
                //5. If emailverified is false resend email invitation otherwise throw error
                if (user.Emailverified)
                    throw new ServiceException("Invitation user fails. User was actived before", HttpStatusCode.NotFound, "User", "Already invited");

                //7. Update invitation data
                user.Email = value.Email;
                user.Name = value.Fullname;
                user.Password = password;
                user.Language = InvitationUtils.GetLanguage(value.Language);
                user.Usertype = InvitationUtils.GetUserType(value.CustomerType);
                user.Invitationtoken = Guid.NewGuid().ToString();
                _userRepository.Update(user);
            }

            //5. Get Email Invitation Template
            EmailTemplate invitationTemplate = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.Invitation, user.Language);
            if (invitationTemplate._id == null)
            {
                invitationTemplate = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.Invitation, LanguageTypes.en.ToString());
            }

            if (invitationTemplate._id != null)
            {
                //6. Send email invitation
                Email message = new Email();
                message.To.Add(user.Email);
                message.Subject = invitationTemplate.subject;
                message.Body = string.Format(invitationTemplate.body, user.Name, user.Username, user.Password,
                    $"{_config["InviteConfirmation"]}{user.Invitationtoken}");
                result = await _mailRepository.Send(message);
            }

            return result;
        }

        public async Task<Token> ConfirmUserAsync(string invitationToken)
        {
            //1. Validate invitationToken not empty
            if (string.IsNullOrEmpty(invitationToken))
                throw new ServiceException("User must have a invitationToken.", HttpStatusCode.BadRequest, "Invitation Token", "Empty field");

            //2. Validate user by invitationToken
            User user = _userRepository.GetUserByInvitationToken(invitationToken);
            if (user.Id == null)
                return new Token();

            //3. Get UserProfile from external system
            ProfilePermissions profilepermissions = await _profileRepository.GetProfilePermissionsAsync(user.Dni);
            string role = Role.User;
            if (profilepermissions.CanManageAccounts)
                role = Role.Admin;

            //4. Create user in Authentication System
            UserIdentity userIdentity = new UserIdentity();
            userIdentity.UserName = user.Username;
            userIdentity.Password = user.Password;
            userIdentity.Emails = new List<string>()
            {
                user.Email
            };
            userIdentity.CardId = user.Dni;
            userIdentity.Language = user.Language;
            userIdentity.DisplayName = user.Name;
            UserIdentity newUser = await _identityRepository.AddUser(userIdentity);

            //5 AddUserToGroup
            GroupResults group = await _identityRepository.FindGroup(role);
            if (group.TotalResults == 1)
                await _identityRepository.AddUserToGroup(newUser, group.Groups[0]);

            //6. Update email verification data
            user.Emailverified = true;
            user.Invitationtoken = null;
            user.ExternalId = newUser.ID;
            _userRepository.Update(user);

            //7. Confirm access status to external system
            await _profileRepository.ConfirmedWebPortalAccessAsync(user.Dni);

            //8. Get Access Token
            Token accessToken = await _identityRepository.Authorize(new Login()
            {
                Username = user.Username,
                Password = user.Password
            });

            return accessToken;
        }

        public Task<bool> UnInviteUserAsync(string dni)
        {
            //1. Validate dni not empty
            if (string.IsNullOrEmpty(dni))
                throw new ServiceException("User must have a valid document number.", HttpStatusCode.BadRequest, "Dni", "Empty field");

            //2. Validate user
            User user = _userRepository.GetCurrentUserByDni(dni);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //3. If emailverified is false
            if (!user.Emailverified)
                return Task.FromResult(false);

            //4. Confirm revocation access status to external system
            _profileRepository.RevokedWebPortalAccessAsync(user.Dni);

            //5. Update invitation data
            user.Emailverified = false;
            user.Invitationtoken = null;
            _userRepository.Update(user);

            //6. Delete from IS
            _identityRepository.DeleteUser(user.ExternalId);

            return Task.FromResult(true);
        }

        public async Task<Account> GetAccountAsync(string dni)
        {
            //Invoke repository
            AccountCrm entity = await _profileRepository.GetAccountAsync(dni);
            if (entity == null)
                throw new ServiceException("Account is not found.", HttpStatusCode.NotFound, "Account", "Not exist");

            var account = ToAccount(entity);

            return account;
        }

        public async Task<Account> UpdateAccountAsync(Account value)
        {
            //Invoke repository
            var accountCrm = new AccountCrm
            {
                SmCustomerId = value.SmCustomerId,
                Phone1 = value.Phone1,
                MobilePhone1 = value.Mobile1,
                EmailAddress1 = value.Email1,
                EmailAddress2 = value.Email2,
                UseThisAddress = value.UseThisAddress
            };

            foreach (var address in value.AddressList)
            {
                if (address.Type == AddressTypes.Main.ToString())
                {
                    accountCrm.Address1Street1 = address.Street1;
                    accountCrm.Address1Street2 = address.Street2;
                    accountCrm.Address1Street3 = address.Street3;
                    accountCrm.Address1City = address.City;
                    accountCrm.Address1StateOrProvince = address.StateOrProvince;
                    accountCrm.Address1PostalCode = address.ZipOrPostalCode;
                    accountCrm.Address1Country = address.Country;
                }
                if (address.Type == AddressTypes.Invoice.ToString())
                {
                    accountCrm.Address2Street1 = address.Street1;
                    accountCrm.Address2Street2 = address.Street2;
                    accountCrm.Address2Street3 = address.Street3;
                    accountCrm.Address2City = address.City;
                    accountCrm.Address2StateOrProvince = address.StateOrProvince;
                    accountCrm.Address2PostalCode = address.ZipOrPostalCode;
                    accountCrm.Address2Country = address.Country;
                }
                if (address.Type == AddressTypes.Alternate.ToString())
                {
                    accountCrm.AlternateStreet1 = address.Street1;
                    accountCrm.AlternateStreet2 = address.Street2;
                    accountCrm.AlternateStreet3 = address.Street3;
                    accountCrm.AlternateCity = address.City;
                    accountCrm.AlternateStateOrProvince = address.StateOrProvince;
                    accountCrm.AlternatePostalCode = address.ZipOrPostalCode;
                    accountCrm.AlternateCountry = address.Country;
                }
            }

            AccountCrm entity = await _profileRepository.UpdateAccountAsync(accountCrm);
            if (entity == null)
                throw new ServiceException("Account is not found.", HttpStatusCode.NotFound, "Account", "Not exist");

            var account = ToAccount(entity);

            return account;
        }

        public async Task<bool> ContactAsync(FormContact value)
        {
            if (string.IsNullOrEmpty(value.Type))
                throw new ServiceException("FormContact Type field can not be null.", HttpStatusCode.BadRequest, "Type", "Empty fields");

            //1. Get currentUser
            var currentUser = Thread.CurrentPrincipal;
            if (currentUser == null)
                throw new ServiceException("Error retrieving the current user.", HttpStatusCode.NotFound, "Current user", "Not logged");

            User user = _userRepository.GetCurrentUser(currentUser.Identity.Name);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            Profile userProfile = await _profileRepository.GetProfileAsync(user.Dni);
            if (userProfile.DocumentNumber == null)
                throw new ServiceException("User Profile does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            Enum.TryParse(typeof(ContactTypes), value.Type, true, out var option);
            Email emailMessage = null;
            switch (option)
            {
                case ContactTypes.Opinion:
                    //2. Check required fields
                    if (value.Message == null)
                        throw new ServiceException("FormContact Message field can not be null.", HttpStatusCode.BadRequest, "Message", "Empty fields");

                    //3. Send Email
                    emailMessage = GenerateEmail(EmailTemplateTypes.FormOpinion, user, userProfile, value);

                    break;
                case ContactTypes.Call:
                    //2. Check required fields
                    if (value.Preference == null)
                        throw new ServiceException("FormContact Preference field can not be null.", HttpStatusCode.BadRequest, "Preference", "Empty fields");

                    if (value.Message == null)
                        throw new ServiceException("FormContact Message field can not be null.", HttpStatusCode.BadRequest, "Message", "Empty fields");

                    //3. Send Email
                    emailMessage = GenerateEmail(EmailTemplateTypes.FormCall, user, userProfile, value);

                    break;
                case ContactTypes.Contact:
                    //2. Check required fields
                    if (value.Motive == null)
                        throw new ServiceException("FormContact Motive field can not be null.", HttpStatusCode.BadRequest, "Motive", "Empty fields");

                    if (value.Message == null)
                        throw new ServiceException("FormContact Message field can not be null.", HttpStatusCode.BadRequest, "Message", "Empty fields");

                    //3. Send Email
                    value.EmailTo = _config["FormContactEmail"];
                    emailMessage = GenerateEmail(EmailTemplateTypes.FormContact, user, userProfile, value);
                    
                    break;
            }

            var result = await _mailRepository.Send(emailMessage);

            return result;
        }

        private static Account ToAccount(AccountCrm entity)
        {
            return new Account
            {
                SmCustomerId = entity.SmCustomerId,
                Phone1 = entity.Phone1,
                Mobile1 = entity.MobilePhone1,
                Email1 = entity.EmailAddress1,
                Email2 = entity.EmailAddress2,
                UseThisAddress = entity.UseThisAddress,
                AddressList = new List<Address>
                {
                    new Address
                    {
                        Street1 = entity.Address1Street1,
                        Street2 = entity.Address1Street2,
                        Street3 = entity.Address1Street3,
                        City = entity.Address1City,
                        StateOrProvince = entity.Address1StateOrProvince,
                        ZipOrPostalCode = entity.Address1PostalCode,
                        Country = entity.Address1Country,
                        Type = AddressTypes.Main.ToString()
                    },
                    new Address
                    {
                        Street1 = entity.Address2Street1,
                        Street2 = entity.Address2Street2,
                        Street3 = entity.Address2Street3,
                        City = entity.Address2City,
                        StateOrProvince = entity.Address2StateOrProvince,
                        ZipOrPostalCode = entity.Address2PostalCode,
                        Country = entity.Address2Country,
                        Type = AddressTypes.Invoice.ToString()
                    },
                    new Address
                    {
                        Street1 = entity.AlternateStreet1,
                        Street2 = entity.AlternateStreet2,
                        Street3 = entity.AlternateStreet3,
                        City = entity.AlternateCity,
                        StateOrProvince = entity.AlternateStateOrProvince,
                        ZipOrPostalCode = entity.AlternatePostalCode,
                        Country = entity.AlternateCountry,
                        Type = AddressTypes.Alternate.ToString()
                    }
                }
            };
        }

        private Email GenerateEmail(EmailTemplateTypes type, User user, Profile userProfile, FormContact form)
        {
            //3. Get Email Invitation Template
            EmailTemplate formContactTemplate = _emailTemplateRepository.getTemplate((int)type, user.Language);
            if (formContactTemplate._id == null)
            {
                formContactTemplate = _emailTemplateRepository.getTemplate((int)type, LanguageTypes.en.ToString());
            }

            if (formContactTemplate._id == null)
                throw new ServiceException("EmailTemplate not found.", HttpStatusCode.BadRequest, "EmailTemplate", "Not found");

            //5. Send email
            var message = new Email();
            message.To.Add(form.EmailTo);
            message.Subject = formContactTemplate.subject;

            string body;
            switch (type)
            {
                case EmailTemplateTypes.FormContact:
                    body = string.Format(
                        formContactTemplate.body,
                        userProfile.Fullname,
                        userProfile.MobilePhone,
                        user.Email,
                        form.Motive,
                        form.Message);
                    break;
                case EmailTemplateTypes.FormOpinion:
                    body = string.Format(
                        formContactTemplate.body,
                        userProfile.Fullname,
                        userProfile.MobilePhone,
                        user.Email,
                        form.Message);
                    break;
                default:
                    body = string.Format(
                        formContactTemplate.body,
                        userProfile.Fullname,
                        userProfile.MobilePhone,
                        user.Email,
                        form.Preference,
                        form.Message);
                    break;
            }
            message.Body = body;

            return message;
        }
    }
}
