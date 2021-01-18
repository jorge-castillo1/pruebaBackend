﻿using customerportalapi.Repositories.interfaces;
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
        private readonly ILoginService _loginService;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly ILanguageRepository _languageRepository;

        public UserServices(
            IUserRepository userRepository,
            IProfileRepository profileRepository,
            IMailRepository mailRepository,
            IEmailTemplateRepository emailTemplateRepository,
            IIdentityRepository identityRepository,
            IConfiguration config,
            ILoginService loginService,
            IUserAccountRepository userAccountRepository,
            ILanguageRepository languageRepository
        )
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _mailRepository = mailRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _identityRepository = identityRepository;
            _config = config;
            _loginService = loginService;
            _userAccountRepository = userAccountRepository;
            _languageRepository = languageRepository;
        }


        public async Task<Profile> GetProfileAsync(string username)
        {
            //Add customer portal Business Logic

            User user = _userRepository.GetCurrentUserByUsername(username);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //1. If emailverified is false throw error
            if (!user.Emailverified)
                throw new ServiceException("User is deactivated,", HttpStatusCode.Forbidden, "User", "Deactivated");

            //2. If exist complete data from external repository
            //Invoke repository
            string accountType = (user.Usertype == (int)UserTypes.Business) ? AccountType.Business : AccountType.Residential;
            Profile entity = await _profileRepository.GetProfileAsync(user.Dni, accountType);

            List<Language> languages = await _languageRepository.GetLanguagesAsync();
            Language langEntity = languages.Find(x => x.Name == entity.Language);
            if (user.Language != langEntity.IsoCode.ToLower()) {
                user.Language = langEntity.IsoCode.ToLower();
                _userRepository.Update(user);
            }

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
            entity.Username = username;
            //entity.CustomerTypeInfo = new AccountCustomerType()
            //{
            //    CustomerType = accountType
            //}

            return entity;
        }

        public async Task<Profile> GetProfileByDniAndTypeAsync(string dni, string accountType)
        {
            //Add customer portal Business Logic
            int userType = UserUtils.GetUserType(accountType);

            User user = _userRepository.GetCurrentUserByDniAndType(dni, userType);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //1. If emailverified is false throw error
            if (!user.Emailverified)
                throw new ServiceException("User is deactivated,", HttpStatusCode.Forbidden, "User", "Deactivated");

            //2. If exist complete data from external repository
            //Invoke repository
            var entity = await _profileRepository.GetProfileAsync(user.Dni, accountType);
            entity.Username = user.Username;

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
            //entity.CustomerTypeInfo = new AccountCustomerType()
            //{
            //    CustomerType = accountType
            //}

            return entity;
        }

        public async Task<Profile> UpdateProfileAsync(Profile profile)
        {
            //Add customer portal Business Logic
            if (profile.CustomerTypeInfo == null)
                profile.CustomerTypeInfo = new AccountCustomerType();

            if (string.IsNullOrEmpty(profile.CustomerTypeInfo.CustomerType))
                profile.CustomerTypeInfo.CustomerType = AccountType.Residential;

            int userType = UserUtils.GetUserType(profile.CustomerTypeInfo.CustomerType);
            User user = _userRepository.GetCurrentUserByDniAndType(profile.DocumentNumber, userType);
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
       
            //3. Verify that principal email not in use
           
            if (profile.EmailAddress1Principal && !string.IsNullOrEmpty(profile.EmailAddress1)) 
            {   
                if (this.VerifyDisponibilityEmail(profile.EmailAddress1, profile.DocumentNumber))
                    throw new ServiceException("Principal email are in use by another user.", HttpStatusCode.BadRequest, "Principal email", "In use");
            }
            if (profile.EmailAddress2Principal && !string.IsNullOrEmpty(profile.EmailAddress2))
            {
                if (this.VerifyDisponibilityEmail(profile.EmailAddress2, profile.DocumentNumber))
                    throw new ServiceException("Principal email are in use by another user.", HttpStatusCode.BadRequest, "Principal email", "In use");
            }

            var emailToUpdate = profile.EmailAddress1Principal ? profile.EmailAddress1 : profile.EmailAddress2;

            //4. Set Phone Principal according to data
            string phoneToUpdate = string.Empty;
            if (profile.MobilePhone1Principal && !string.IsNullOrEmpty(profile.MobilePhone1))
                phoneToUpdate = profile.MobilePhone1;
            else if (profile.MobilePhonePrincipal && !string.IsNullOrEmpty(profile.MobilePhone))
                phoneToUpdate = profile.MobilePhone;

            //5. Compare language, email and image for backend changes
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

            //6. Invoke repository for other changes
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

            EmailTemplate editDataCustomerTemplate = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.EditDataCustomer, user.Language);

            if (editDataCustomerTemplate._id != null)
            {
                Email message = new Email();
                message.To.Add(user.Email);
                message.Subject = editDataCustomerTemplate.subject;
                string htmlbody = editDataCustomerTemplate.body.Replace("{", "{{").Replace("}", "}}").Replace("%{{", "{").Replace("}}%", "}");
                message.Body = string.Format(htmlbody, user.Name);
                await _mailRepository.Send(message);
            }

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
            var userType = UserUtils.GetUserType(value.CustomerType);
            var userName = userType == 0 ? value.Dni : "B" + value.Dni;

            var pwd = new Password(true, true, true, false, 6);
            var password = pwd.Next();

            //3.1 Find some user with this email or without confirm email
            User user = _userRepository.GetCurrentUserByEmail(value.Email);
            if (user.Id != null && user.Emailverified) 
                throw new ServiceException("Invitation user fails. Email in use by another user", HttpStatusCode.NotFound, "Email", "Already in use");
                        
            user = _userRepository.GetCurrentUserByDniAndType(value.Dni, userType);
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
                    Language = UserUtils.GetLanguage(value.Language),
                    Usertype = UserUtils.GetUserType(value.CustomerType),
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
                user.Language = UserUtils.GetLanguage(value.Language);
                user.Usertype = UserUtils.GetUserType(value.CustomerType);
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
                string htmlbody = invitationTemplate.body.Replace("{", "{{").Replace("}", "}}").Replace("%{{", "{").Replace("}}%", "}");
                message.Body = string.Format(htmlbody, user.Name, user.Username, user.Password,
                    $"{_config["InviteConfirmation"]}{user.Invitationtoken}");
                result = await _mailRepository.Send(message);
            }

            return result;
        }

        public async Task<Token> ConfirmAndChangeCredentialsAsync(string receivedToken, ResetPassword value)
        {
            // 1. Validate user
            if (string.IsNullOrEmpty(receivedToken)) throw new ServiceException("User must have a received Token.", HttpStatusCode.BadRequest, "Received Token", "Empty field");

            User user = _userRepository.GetUserByInvitationToken(receivedToken);
            if (user.Id == null) return new Token();

            if (user.Password != value.OldPassword) throw new ServiceException("Wrong password.", HttpStatusCode.BadRequest);
            user.Password = value.NewPassword;

            // Get UserProfile from external system
            string accountType = UserUtils.GetAccountType(user.Usertype);
            ProfilePermissions profilepermissions = await _profileRepository.GetProfilePermissionsAsync(user.Dni, accountType);
            string role = Role.User;
            if (profilepermissions.CanManageAccounts) role = Role.Admin;

            // 2. Change useranme
            if (value.Username != null && value.Username != "")
            {
                if(value.Username.Contains('@'))
                    throw new ServiceException("Username must not include @", HttpStatusCode.BadRequest, "Username", "Must not include @"); // TODO: test postman

                if (ValidateUsername(value.Username)) user.Username = value.Username;
                else throw new ServiceException("Username must be unique", HttpStatusCode.BadRequest, "Username", "Must be unique");
            }

            // 3. Afegir-lo a l'IS
            UserIdentity newUser = await AddUserToIdentityServer(user);

            GroupResults group = await _identityRepository.FindGroup(role);
            if (group.TotalResults == 1)
                await _identityRepository.AddUserToGroup(newUser, group.Groups[0]);

            user.Password = null;
            user.Emailverified = true;
            user.Invitationtoken = null;
            user.ExternalId = newUser.ID;
            _userRepository.UpdateById(user);

            // Confirm access status to external system
            await _profileRepository.ConfirmedWebPortalAccessAsync(user.Dni, accountType);

            //8. Get Access Token
            Token accessToken = await _identityRepository.Authorize(new Login()
            {
                Username = user.Username,
                Password = value.NewPassword
            });

            return accessToken;
        }

        private async Task<UserIdentity> AddUserToIdentityServer(User user)
        {
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
            return await _identityRepository.AddUser(userIdentity);
        }

        public async Task<Token> ConfirmUserAsync(string receivedToken)
        {
            //1. Validate receivedToken not empty
            if (string.IsNullOrEmpty(receivedToken))
                throw new ServiceException("User must have a receivedToken.", HttpStatusCode.BadRequest, "Received Token", "Empty field");

            //2. Validate user by invitationToken or forgotPasswordToken
            bool invitationToken = false;
            bool forgotPasswordToken = false;
            User user = _userRepository.GetUserByInvitationToken(receivedToken);
            if (user.Id != null)
                invitationToken = true;
            else
            {
                user = _userRepository.GetUserByForgotPasswordToken(receivedToken);
                if (user.Id != null)
                    forgotPasswordToken = true;
                else
                    return new Token();
            }

            if (invitationToken)
            {
                //3. Get UserProfile from external system
                string accountType = UserUtils.GetAccountType(user.Usertype);
                ProfilePermissions profilepermissions = await _profileRepository.GetProfilePermissionsAsync(user.Dni, accountType);
                string role = Role.User;
                if (profilepermissions.CanManageAccounts)
                    role = Role.Admin;

                //4. Create user in Authentication System
                UserIdentity newUser = await AddUserToIdentityServer(user);

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
                await _profileRepository.ConfirmedWebPortalAccessAsync(user.Dni, accountType);
            }
            else if (forgotPasswordToken)
            {
                //3. Update user
                UserIdentity existingUser = await _identityRepository.GetUser(user.ExternalId);
                if (existingUser != null)
                {
                    existingUser.Password = user.Password;
                    await _identityRepository.UpdateUser(existingUser);
                }

                //4. Update verification data
                user.ForgotPasswordtoken = null;
                _userRepository.Update(user);
            }
            else
            {
                return new Token();
            }
            //8. Get Access Token
            Token accessToken = await _identityRepository.Authorize(new Login()
            {
                Username = user.Username,
                Password = user.Password
            });

            return accessToken;
        }

        public Task<bool> UnInviteUserAsync(Invitation value)
        {
            //1. Validate dni not empty
            if (string.IsNullOrEmpty(value.Dni))
                throw new ServiceException("User must have a valid document number.", HttpStatusCode.BadRequest, "Dni", "Empty field");

            //2. Validate user
            int userType = UserUtils.GetUserType(value.CustomerType);
            User user = _userRepository.GetCurrentUserByDniAndType(value.Dni, userType);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //3. If emailverified is false
            if (!user.Emailverified)
                return Task.FromResult(false);

            //4. Confirm revocation access status to external system
            _profileRepository.RevokedWebPortalAccessAsync(user.Dni, value.CustomerType);

            //5. Update invitation data
            user.Emailverified = false;
            user.Invitationtoken = null;
            _userRepository.Update(user);

            //6. Delete from IS
            _identityRepository.DeleteUser(user.ExternalId);

            //7. Delete from Database
            _userRepository.Delete(user);

            return Task.FromResult(true);
        }

        public async Task<Account> GetAccountAsync(string username)
        {
            User user = _userRepository.GetCurrentUserByUsername(username);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //Invoke repository
            string accountType = UserUtils.GetAccountType(user.Usertype);
            AccountProfile entity = await _profileRepository.GetAccountAsync(user.Dni, accountType);
            UserAccount userAccount = _userAccountRepository.GetAccount(username);
            if (userAccount.Profilepicture != null)
                entity.Profilepicture = userAccount.Profilepicture;

            if (entity == null)
                throw new ServiceException("Account is not found.", HttpStatusCode.NotFound, "Account", "Not exist");

            var account = ToAccount(entity);

            return account;
        }

        public async Task<Account> GetAccountByDocumentNumberAsync(string documentNumber)
        {
            AccountProfile accountProfile = await _profileRepository.GetAccountByDocumentNumberAsync(documentNumber);
            if (accountProfile.DocumentNumber == null)
                throw new ServiceException("Account does not exist.", HttpStatusCode.NotFound, "DocumentNumber", "Not exist");

            var account = ToAccount(accountProfile);

            return account;
        }

        public async Task<Account> UpdateAccountAsync(Account value, string username)
        {
            //Invoke repository
            var accountprofile = new AccountProfile
            {
                SmCustomerId = value.SmCustomerId,
                CompanyName = value.CompanyName,
                Phone1 = value.Phone1,
                MobilePhone1 = value.Mobile1,
                EmailAddress1 = value.Email1,
                EmailAddress2 = value.Email2,
                UseThisAddress = value.UseThisAddress,
                Token = value.Token,
                TokenUpdateDate = value.TokenUpdateDate,
                BankAccount = value.BankAccount
            };

            foreach (var address in value.AddressList)
            {
                if (address.Type == AddressTypes.Main.ToString())
                {
                    accountprofile.Address1Street1 = address.Street1;
                    accountprofile.Address1Street2 = address.Street2;
                    accountprofile.Address1Street3 = address.Street3;
                    accountprofile.Address1City = address.City;
                    accountprofile.Address1StateOrProvince = address.StateOrProvince;
                    accountprofile.Address1PostalCode = address.ZipOrPostalCode;
                    accountprofile.Address1Country = address.Country;
                }
                if (address.Type == AddressTypes.Invoice.ToString())
                {
                    accountprofile.Address2Street1 = address.Street1;
                    accountprofile.Address2Street2 = address.Street2;
                    accountprofile.Address2Street3 = address.Street3;
                    accountprofile.Address2City = address.City;
                    accountprofile.Address2StateOrProvince = address.StateOrProvince;
                    accountprofile.Address2PostalCode = address.ZipOrPostalCode;
                    accountprofile.Address2Country = address.Country;
                }
                if (address.Type == AddressTypes.Alternate.ToString())
                {
                    accountprofile.AlternateStreet1 = address.Street1;
                    accountprofile.AlternateStreet2 = address.Street2;
                    accountprofile.AlternateStreet3 = address.Street3;
                    accountprofile.AlternateCity = address.City;
                    accountprofile.AlternateStateOrProvince = address.StateOrProvince;
                    accountprofile.AlternatePostalCode = address.ZipOrPostalCode;
                    accountprofile.AlternateCountry = address.Country;
                }
            }

            AccountProfile entity = await _profileRepository.UpdateAccountAsync(accountprofile);
            if (entity == null)
                throw new ServiceException("Account is not found.", HttpStatusCode.NotFound, "Account", "Not exist");


            UserAccount userAccount = _userAccountRepository.GetAccount(username);


            if (userAccount.Id == null)
            {
                UserAccount newUserAccount = new UserAccount()
                {
                    Username = username,
                    Profilepicture = value.Profilepicture

                };
                bool create = await _userAccountRepository.Create(newUserAccount);
                if (create == true)
                    entity.Profilepicture = value.Profilepicture;

            }
            else
            {
                UserAccount userAccountToUpdate = new UserAccount()
                {
                    Id = userAccount.Id,
                    Username = username,
                    Profilepicture = value.Profilepicture

                };
                UserAccount create = _userAccountRepository.Update(userAccountToUpdate);
                if (create.Profilepicture != null)
                    entity.Profilepicture = value.Profilepicture;
            }

            var account = ToAccount(entity);
            User user = _userRepository.GetCurrentUserByUsername(username);
            EmailTemplate editDataCustomerTemplate = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.EditDataCustomer, user.Language);

            if (editDataCustomerTemplate._id != null)
            {
                if (user.Id != null) {
                    Email message = new Email();
                    message.To.Add(user.Email);
                    message.Subject = editDataCustomerTemplate.subject;
                    string htmlbody = editDataCustomerTemplate.body.Replace("{", "{{").Replace("}", "}}").Replace("%{{", "{").Replace("}}%", "}");
                    message.Body = string.Format(htmlbody, user.Name);
                    await _mailRepository.Send(message);
                }

            }

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

            User user = _userRepository.GetCurrentUserByUsername(currentUser.Identity.Name);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            string accountType = UserUtils.GetAccountType(user.Usertype);
            Profile userProfile = await _profileRepository.GetProfileAsync(user.Dni, accountType);
            if (userProfile.DocumentNumber == null)
                throw new ServiceException("User Profile does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            Enum.TryParse(typeof(ContactTypes), value.Type, true, out var option);
            Email emailMessage = null;
            Email customerEmailMessage = null;
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

                    if (value.ContactMethod == null)
                        throw new ServiceException("FormContact Message field can not be null.", HttpStatusCode.BadRequest, "ContactMethod", "Empty fields");

                    //3. Send Email
                    emailMessage = GenerateEmail(EmailTemplateTypes.FormCall, user, userProfile, value);
                    customerEmailMessage = GenerateEmail(EmailTemplateTypes.FormCallCustomer, user, userProfile, value);
                    await _mailRepository.Send(customerEmailMessage);

                    break;
                case ContactTypes.Contact:
                    //2. Check required fields
                    if (value.Motive == null)
                        throw new ServiceException("FormContact Motive field can not be null.", HttpStatusCode.BadRequest, "Motive", "Empty fields");

                    if (value.Message == null)
                        throw new ServiceException("FormContact Message field can not be null.", HttpStatusCode.BadRequest, "Message", "Empty fields");

                    if (value.EmailTo == null)
                        throw new ServiceException("FormContact Message field can not be null.", HttpStatusCode.BadRequest, "EmailTo", "Empty fields");

                    //3. Send Email
                    emailMessage = GenerateEmail(EmailTemplateTypes.FormContact, user, userProfile, value);
                    customerEmailMessage = GenerateEmail(EmailTemplateTypes.FormContactCustomer, user, userProfile, value);
                    await _mailRepository.Send(customerEmailMessage);
                    break;
            }

            var result = await _mailRepository.Send(emailMessage);
            return result;
        }

        private static Account ToAccount(AccountProfile entity)
        {
            return new Account
            {
                SmCustomerId = entity.SmCustomerId,
                CompanyName = entity.CompanyName,
                DocumentNumber = entity.DocumentNumber,
                Phone1 = entity.Phone1,
                Mobile1 = entity.MobilePhone1,
                Email1 = entity.EmailAddress1,
                Email2 = entity.EmailAddress2,
                UseThisAddress = entity.UseThisAddress,
                CustomerType = entity.CustomerType,
                Profilepicture = entity.Profilepicture,
                PaymentMethodId = entity.PaymentMethodId,
                BankAccount = entity.BankAccount,
                Token = entity.Token,
                TokenUpdateDate = entity.TokenUpdateDate,
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
            string email = form.EmailTo;
            if (!(_config["Environment"] == nameof(EnvironmentTypes.PRO))) email = _config["MailStores"];

            message.To.Add(email);
            message.Cc.Add(user.Email);
            message.Subject = formContactTemplate.subject;
            string body;
            string htmlbody;
            switch (type)
            {
                case EmailTemplateTypes.FormContact:
                    body = string.Format(
                        formContactTemplate.body,
                        userProfile.Fullname,
                        userProfile.MobilePhone,
                        userProfile.MobilePhone1,
                        user.Email,
                        form.Motive,
                        form.Message,
                        form.Preference,
                        form.ContactMethod);
                    break;
                case EmailTemplateTypes.FormContactCustomer:
                    htmlbody = formContactTemplate.body.Replace("{", "{{").Replace("}", "}}").Replace("%{{", "{").Replace("}}%", "}");
                    body = string.Format(
                        htmlbody,
                        userProfile.Fullname,
                        form.MotiveValue,
                        form.Message);
                    break;
                case EmailTemplateTypes.FormOpinion:
                    body = string.Format(
                        formContactTemplate.body,
                        userProfile.Fullname,
                        userProfile.MobilePhone,
                        userProfile.MobilePhone1,
                        user.Email,
                        form.Message);
                    break;
                case EmailTemplateTypes.FormCallCustomer:
                    htmlbody = formContactTemplate.body.Replace("{", "{{").Replace("}", "}}").Replace("%{{", "{").Replace("}}%", "}");
                    body = string.Format(
                        htmlbody,
                        userProfile.Fullname,
                        form.ContactMethodValue,
                        form.PreferenceValue);
                    break;
                default:
                    string date = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    body = string.Format(
                        formContactTemplate.body,
                        userProfile.Fullname,
                        userProfile.MobilePhone,
                        userProfile.MobilePhone1,
                        user.Email,
                        user.Dni,
                        form.ContactMethod,
                        form.Preference,
                        form.Message,
                        date);
                    break;
            }
            message.Body = body;

            return message;
        }

        public Profile GetUserByUsername(string username)
        {
            //Invoke repository

            User user = _userRepository.GetCurrentUserByUsername(username);

            Profile profile = ToProfile(user);
            if (profile == null)
                throw new ServiceException("User is not found.", HttpStatusCode.NotFound, "User", "Not exist");

            return profile;
        }

        private static Profile ToProfile(User entity)
        {

            return new Profile
            {
                Fullname = entity.Name,
                DocumentNumber = entity.Dni,
                Username = entity.Username
            };
        }

        public async Task<bool> ChangeRole(string username, string role)
        {
            User user = _userRepository.GetCurrentUserByUsername(username);
            UserIdentity userIdentity = await _identityRepository.GetUser(user.ExternalId);
            if (userIdentity.ID == null) throw new ServiceException("User not found", HttpStatusCode.NotFound);
            GroupResults group = await _identityRepository.FindGroup(role);
            if (group.Groups.Count == 0) throw new ServiceException("Role not found", HttpStatusCode.NotFound);
            if (userIdentity.Groups != null)
            {
                foreach (var oldGroup in userIdentity.Groups)
                {
                    GroupResults currentGroup = await _identityRepository.FindGroup(oldGroup.Display);
                    await _identityRepository.RemoveUserFromGroup(userIdentity, currentGroup.Groups[0]);
                }
            }
            await _identityRepository.AddUserToGroup(userIdentity, group.Groups[0]);
            return true;
        }

        public async Task<bool> RemoveRole(string username, string role)
        {
            User user = _userRepository.GetCurrentUserByUsername(username);
            UserIdentity userIdentity = await _identityRepository.GetUser(user.ExternalId);
            if (userIdentity.ID == null) throw new ServiceException("User not found", HttpStatusCode.NotFound);
            if (userIdentity.Groups == null || !userIdentity.Groups.Exists(x => x.Display == role)) return false;
            GroupResults group = await _identityRepository.FindGroup(role);
            return await _identityRepository.RemoveUserFromGroup(userIdentity, group.Groups[0]);
        }

        public bool ValidateUsername(string username)
        {
            User user = _userRepository.GetCurrentUserByUsername(username);
            return (user.Id == null);
        }

        private bool VerifyDisponibilityEmail(string email, string dni)
        {
            User userByEmail = _userRepository.GetCurrentUserByEmail(email);
            return (userByEmail.Id != null && userByEmail.Dni != dni);
        }
    }
}
