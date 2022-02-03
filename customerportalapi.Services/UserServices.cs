﻿using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using PasswordGenerator;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly IContractRepository _contractRepository;
        private readonly IContractSMRepository _contractSMRepository;
        private readonly IOpportunityCRMRepository _opportunityRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IUnitLocationRepository _unitLocationRepository;
        private readonly IFeatureRepository _featureRepository;
        private readonly INewUserRepository _newUserRepository;
        private readonly IGoogleCaptchaRepository _googleCaptchaRepository;


        public UserServices(
            IUserRepository userRepository,
            IProfileRepository profileRepository,
            IMailRepository mailRepository,
            IEmailTemplateRepository emailTemplateRepository,
            IIdentityRepository identityRepository,
            IConfiguration config,
            ILoginService loginService,
            IUserAccountRepository userAccountRepository,
            ILanguageRepository languageRepository,
            IContractRepository contractRepository,
            IContractSMRepository contractSMRepository,
            IOpportunityCRMRepository opportunityRepository,
            IStoreRepository storeRepository,
            IUnitLocationRepository unitLocationRepository,
            IFeatureRepository featureRepository,
            INewUserRepository newUserRepository,
            IGoogleCaptchaRepository googleCaptchaRepository
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
            _contractRepository = contractRepository;
            _contractSMRepository = contractSMRepository;
            _opportunityRepository = opportunityRepository;
            _storeRepository = storeRepository;
            _unitLocationRepository = unitLocationRepository;
            _featureRepository = featureRepository;
            _newUserRepository = newUserRepository;
            _googleCaptchaRepository = googleCaptchaRepository;
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
            if (user.Language != langEntity.IsoCode.ToLower())
            {
                user.Language = langEntity.IsoCode.ToLower();
                _userRepository.Update(user);
            }

            //3. Set Email Principal according to external data. No two principal emails allowed

            if (string.IsNullOrEmpty(user.Email) || user.Email != entity.EmailAddress1)
            {
                if (VerifyDisponibilityEmail(entity.EmailAddress1, entity.DocumentNumber))
                {
                    EmailTemplate template = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.ErrorChangeEmailAlreadyExists, LanguageTypes.es.ToString());
                    if (string.IsNullOrEmpty(template._id))
                        throw new ServiceException("Email template not found, templateCode: " + (int)EmailTemplateTypes.ErrorChangeEmailAlreadyExists, HttpStatusCode.NotFound, FieldNames.Email + FieldNames.Template, ValidationMessages.NotFound);

                    string mailTo = _config["MailIT"];
                    if (string.IsNullOrEmpty(mailTo))
                        throw new ServiceException("Store mail not found", HttpStatusCode.NotFound, FieldNames.Email, ValidationMessages.NotFound);

                    Email message = new Email();
                    message.To.Add(mailTo);
                    message.Subject = template.subject;
                    message.Body = string.Format(string.Format(template.body, user.Name, user.Dni));

                    await _mailRepository.Send(message);
                }
                else
                {
                    user.Email = entity.EmailAddress1;
                }
            }

            if (string.IsNullOrEmpty(user.Phone) || user.Phone != entity.MobilePhone)
                user.Phone = entity.MobilePhone;

            if (string.IsNullOrEmpty(user.Name) || user.Name != entity.Name)
                user.Name = entity.Name;

            entity.Language = user.Language;
            entity.Avatar = user.Profilepicture;
            entity.Username = username;

            //entity.CustomerTypeInfo = new AccountCustomerType()
            //{
            //    CustomerType = accountType
            //}

            //5. Update verification data
            user.ForgotPasswordtoken = null;
            _userRepository.Update(user);

            return entity;
        }

        public async Task<Profile> GetProfileByDniAndTypeAsync(string dni, string accountType)
        {
            //Add customer portal Business Logic
            int userType = UserInvitationUtils.GetUserType(accountType);

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
            //entity.MobilePhone1Principal = false;
            //entity.MobilePhonePrincipal = false;

            //if (entity.MobilePhone1 == user.Phone && !string.IsNullOrEmpty(user.Phone))
            //    entity.MobilePhone1Principal = true;
            //else if (entity.MobilePhone == user.Phone && !string.IsNullOrEmpty(user.Phone))
            //    entity.MobilePhonePrincipal = true;

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

            User user = _userRepository.GetCurrentUserByUsername(profile.Username);

            if (user.Usertype == 1)
            {
                profile.CustomerTypeInfo.CustomerType = AccountType.Business;
            }
            else
            {
                profile.CustomerTypeInfo.CustomerType = AccountType.Residential;
            }

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


            //3. Verify that principal email not in use
            if (!profile.EmailAddress1Principal && !string.IsNullOrEmpty(profile.EmailAddress1))
            {
                profile.EmailAddress1Principal = true;
            }
            if (profile.EmailAddress1Principal && !string.IsNullOrEmpty(profile.EmailAddress1))
            {
                if (this.VerifyDisponibilityEmail(profile.EmailAddress1, profile.DocumentNumber))
                    throw new ServiceException("Principal email is in use by another user.", HttpStatusCode.BadRequest, FieldNames.Principalemail, ValidationMessages.InUse);
            }
            if (profile.EmailAddress2Principal && !string.IsNullOrEmpty(profile.EmailAddress2))
            {
                if (this.VerifyDisponibilityEmail(profile.EmailAddress2, profile.DocumentNumber))
                    throw new ServiceException("Email is in use by another user.", HttpStatusCode.BadRequest, FieldNames.Principalemail, ValidationMessages.InUse);
            }

            var emailToUpdate = profile.EmailAddress1Principal ? profile.EmailAddress1 : "Main Email must have a value";

            //4. Set Phone Principal according to data
            string phoneToUpdate = string.Empty;
            if (!string.IsNullOrEmpty(profile.MobilePhone))
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

            //if (entity.MobilePhone1 == user.Phone && !string.IsNullOrEmpty(user.Phone))
            //    entity.MobilePhone1Principal = true;
            //else if (entity.MobilePhone == user.Phone && !string.IsNullOrEmpty(user.Phone))
            //    entity.MobilePhonePrincipal = true;

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

        public async Task<bool> InviteUserAsync(Invitation invitationValues)
        {
            bool resultCreateUser = true;
            bool resultWelcomeEmailSent = false;

            //1. Validate email not empty
            if (string.IsNullOrEmpty(invitationValues.Email))
            {
                throw new ServiceException("User must have a valid email address.", HttpStatusCode.BadRequest, FieldNames.Email, ValidationMessages.EmptyFields);
            }

            //2. Validate dni not empty
            if (string.IsNullOrEmpty(invitationValues.Dni))
            {
                throw new ServiceException("User must have a valid document number.", HttpStatusCode.BadRequest, FieldNames.Dni, ValidationMessages.EmptyFields);
            }

            //3. Find some user with this email and without confirm email
            User user = _userRepository.GetCurrentUserByEmail(invitationValues.Email);
            if (!string.IsNullOrEmpty(user.Id) && user.Emailverified)
            {

                EmailTemplate template = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.ErrorInvitationEmailAlreadyExists, LanguageTypes.es.ToString());
                if (string.IsNullOrEmpty(template._id))
                    throw new ServiceException("Email template not found, templateCode: " + (int)EmailTemplateTypes.ErrorInvitationEmailAlreadyExists, HttpStatusCode.NotFound, FieldNames.Email + FieldNames.Template, ValidationMessages.NotFound);

                string mailTo = _config["MailIT"];
                if (string.IsNullOrEmpty(mailTo))
                    throw new ServiceException("Store mail not found", HttpStatusCode.NotFound, FieldNames.Email, ValidationMessages.NotFound);

                Email message2 = new Email();
                message2.To.Add(mailTo);
                message2.Subject = template.subject;
                message2.Body = string.Format(template.body, user.Name, user.Dni, user.Email);

                await _mailRepository.Send(message2);

                throw new ServiceException("Invitation user fails. Email in use by another user", HttpStatusCode.NotFound, FieldNames.Email, ValidationMessages.AlreadyInUse);
            }

            //4. If emailverified is true throw error
            var userType = UserInvitationUtils.GetUserType(invitationValues.CustomerType);
            user = _userRepository.GetCurrentUserByDniAndType(invitationValues.Dni, userType);
            if (!string.IsNullOrEmpty(user.Id) && user.Emailverified)
            {
                throw new ServiceException("Invitation user fails. User was actived before", HttpStatusCode.NotFound, FieldNames.User, ValidationMessages.AlreadyInvited);
            }

            //5. Get Mandatory data for body email template
            var invitationFields = await FindInvitationMandatoryData(invitationValues);

            //6. Check all mandatory data            
            await CheckMandatoryData(invitationFields);

            //7. Verify "move in date" in range to send remember welcome email (2 days)
            /*if (invitationValues.InvokedBy == (int)InviteInvocationType.CronJob)
            {
                var moveIn = Convert.ToDateTime(invitationFields.ExpectedMoveIn.Value);

                var dateMoveInStart = DateTime.Today.AddDays(2);
                var dateMoveInEnd = DateTime.Today.AddDays(3).AddSeconds(-1);
                
                if (!moveIn.InRange(dateMoveInStart, dateMoveInEnd))
                {                    
                    return false;                    
                }
            }*/

            var userName = userType == 0 ? invitationValues.Dni : "B" + invitationValues.Dni;
            var pwd = new Password(true, true, true, false, 6);
            var password = pwd.Next();
            var isNewUser = false;

            //8. Verify user exsist
            if (user.Id == null)
            {
                isNewUser = true;
                //8.1 Create user in portal database
                user = new User
                {
                    Username = userName,
                    Dni = invitationValues.Dni,
                    Email = invitationValues.Email,
                    Name = invitationValues.Fullname,
                    Password = password,
                    Language = UserInvitationUtils.GetLanguage(invitationValues.Language),
                    Usertype = UserInvitationUtils.GetUserType(invitationValues.CustomerType),
                    Emailverified = false,
                    Invitationtoken = Guid.NewGuid().ToString(),
                    LastEmailSent = EmailTemplateTypes.WelcomeEmailExtended.ToString(),
                };

                resultCreateUser = await _userRepository.Create(user);
            }
            else
            {
                //8.2 Update invitation data
                user.Email = invitationValues.Email;
                user.Name = invitationValues.Fullname;
                user.Password = password;
                user.Language = UserInvitationUtils.GetLanguage(invitationValues.Language);
                user.Usertype = UserInvitationUtils.GetUserType(invitationValues.CustomerType);
                user.Invitationtoken = Guid.NewGuid().ToString();
                user.LastEmailSent = EmailTemplateTypes.WelcomeEmailShort.ToString();

                _userRepository.Update(user);
            }

            //9. Send Welcome Email
            resultWelcomeEmailSent = SendWelcomeEmail(invitationValues, user, invitationFields, isNewUser).Result;

            return resultWelcomeEmailSent && resultCreateUser;
        }

        public async Task<InvitationMandatoryData> FindInvitationMandatoryData(Invitation invitationValues)
        {
            var userType = UserInvitationUtils.GetUserType(invitationValues.CustomerType);
            InvitationMandatoryData invitationFields = UserInvitationUtils.InitInvitationData();
            string accountType = (userType == (int)UserTypes.Business) ? AccountType.Business : AccountType.Residential;
            await FindInvitationMandatoryData(invitationFields, invitationValues, accountType);
            return invitationFields;
        }

        private async Task<bool> SendWelcomeEmail(Invitation invitationValues, User user, InvitationMandatoryData invitationFields, bool isNewUser)
        {
            //Invoke repository
            string accountType = UserInvitationUtils.GetAccountType(user.Usertype);
            AccountProfile entity = await _profileRepository.GetAccountAsync(user.Dni, accountType);
            if (entity == null)
                throw new ServiceException("Account is not found.", HttpStatusCode.NotFound, "Account", "Not exist");

            int templateId = (int)EmailTemplateTypes.WelcomeEmailShort;
            bool useEmailWelcome = _featureRepository.CheckFeatureByNameAndEnvironment(FeatureNames.EmailWelcomeInvitation, _config["Environment"], entity.Address1Country);
            if (isNewUser && useEmailWelcome)
            {
                templateId = (int)EmailTemplateTypes.WelcomeEmailExtended;
            }

            EmailTemplate invitationTemplate = _emailTemplateRepository.getTemplate(templateId, UserInvitationUtils.GetLanguage(invitationValues.Language));
            if (invitationTemplate._id == null)
                invitationTemplate = _emailTemplateRepository.getTemplate(templateId, LanguageTypes.en.ToString());

            if (string.IsNullOrEmpty(invitationTemplate._id))
                throw new ServiceException("Email template not found, templateCode: " + templateId, HttpStatusCode.NotFound, FieldNames.Email + FieldNames.Template, ValidationMessages.NotFound);

            var message = new Email { Subject = invitationTemplate.subject };
            message.To.Add(user.Email);
            message.Body = UserInvitationUtils.GetBodyFormatted(invitationTemplate, user, invitationFields, _config["BaseUrl"], _config["InviteConfirmation"]);

            return await _mailRepository.Send(message);
        }

        public async Task<Token> ConfirmAndChangeCredentialsAsync(string receivedToken, ResetPassword value)
        {
            // 1. Validate user
            if (string.IsNullOrEmpty(receivedToken)) throw new ServiceException("User must have a received Token.", HttpStatusCode.BadRequest, FieldNames.ReceivedToken, ValidationMessages.EmptyFields);

            User user = _userRepository.GetUserByInvitationToken(receivedToken);
            if (user.Id == null) return new Token();

            if (user.Password != value.OldPassword) throw new ServiceException("Wrong password.", HttpStatusCode.BadRequest);
            user.Password = value.NewPassword;

            // Get UserProfile from external system
            string accountType = UserInvitationUtils.GetAccountType(user.Usertype);
            ProfilePermissions profilepermissions = await _profileRepository.GetProfilePermissionsAsync(user.Dni, accountType);
            string role = Role.User;
            if (profilepermissions.CanManageAccounts) role = Role.Admin;

            // 2. Change useranme
            if (value.Username != null && value.Username != "")
            {
                if (value.Username.Contains('@'))
                    throw new ServiceException("Username must not include @", HttpStatusCode.BadRequest, "Username", "Must not include @");

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
                throw new ServiceException("User must have a receivedToken.", HttpStatusCode.BadRequest, FieldNames.ReceivedToken, ValidationMessages.EmptyFields);

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
                string accountType = UserInvitationUtils.GetAccountType(user.Usertype);
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
            int userType = UserInvitationUtils.GetUserType(value.CustomerType);
            User user = _userRepository.GetCurrentUserByDniAndType(value.Dni, userType);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //3. Confirm revocation access status to external system
            _profileRepository.RevokedWebPortalAccessAsync(user.Dni, value.CustomerType);

            //4. Delete from IS
            if (!string.IsNullOrEmpty(user.ExternalId))
                _identityRepository.DeleteUser(user.ExternalId);

            //5. Delete from Database
            _userRepository.Delete(user);

            return Task.FromResult(true);
        }

        public async Task<Account> GetAccountAsync(string username)
        {
            User user = _userRepository.GetCurrentUserByUsername(username);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //Invoke repository
            string accountType = UserInvitationUtils.GetAccountType(user.Usertype);
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
                MobilePhone1 = value.Mobile1,
                Phone1 = value.Phone1,
                EmailAddress1 = value.Email1,
                EmailAddress2 = value.Email2,
                UseThisAddress = value.UseThisAddress,
                Token = value.Token,
                TokenUpdateDate = value.TokenUpdateDate,
                BankAccount = value.BankAccount,
                blue_updatewebportal = true

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
                if (user.Id != null)
                {
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

            string accountType = UserInvitationUtils.GetAccountType(user.Usertype);
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


        private async Task<bool> SendEmailInvitationError(InvitationMandatoryData fields)
        {
            PropertyInfo[] properties = typeof(InvitationMandatoryData).GetProperties();

            EmailTemplate invitationErrorTemplate = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.InvitationError, LanguageTypes.es.ToString());
            if (string.IsNullOrEmpty(invitationErrorTemplate._id))
                throw new ServiceException("Email template not found, templateCode: " + (int)EmailTemplateTypes.InvitationError, HttpStatusCode.NotFound, FieldNames.Email + FieldNames.Template, ValidationMessages.NotFound);

            string mailTo = _config["MailIT"];
            if (string.IsNullOrEmpty(mailTo))
                throw new ServiceException("Store mail not found", HttpStatusCode.NotFound, FieldNames.Email, ValidationMessages.NotFound);

            Email message = new Email();
            message.To.Add(mailTo);
            message.Subject = invitationErrorTemplate.subject;
            message.Body = invitationErrorTemplate.body;
            string list = string.Empty;
            foreach (var property in properties)
            {
                var item = property.GetType();
                MandatoryData data = (MandatoryData)property.GetValue(fields);
                string state = StateEnum.Unchecked.ToString().ToLower();
                string value = string.Empty;
                string system = string.Empty;
                string entity = string.Empty;
                if (data != null)
                {
                    state = data.State.ToString().ToLower();
                    value = data.Value ?? "";
                    system = data.System.ToString() == "empty" ? "-" : data.System.ToString();
                    entity = data.Entity.ToString() == "empty" ? "-" : data.Entity.ToString();
                }
                list += $"<tr class='{state}'><td>{system}</td><td>{entity}</td><td>{property.Name}</td><td>{value}</td></tr>";
            }
            message.Body = message.Body.Replace("{{rows}}", list);

            bool result = await _mailRepository.Send(message);
            return result;
        }

        private async Task<bool> FindInvitationMandatoryData(InvitationMandatoryData invitationData, Invitation value, string accountType)
        {
            invitationData.InvokedBy.SetValueAndState(value.InvokedBy.ToString(), StateEnum.Checked);

            // Contact
            string userIdentification = value.Dni + " - " + accountType;
            Profile contact = await _profileRepository.GetProfileAsync(value.Dni, accountType);
            if (contact == null)
            {
                invitationData.ContactUsername.State = StateEnum.Error;
                await SendEmailInvitationError(invitationData);
                throw new ServiceException("Contact required:  user: " + userIdentification, HttpStatusCode.NotFound, FieldNames.Contact, ValidationMessages.Required);
            }
            invitationData.ContactUsername.SetValueAndState(value.Fullname, StateEnum.Checked);

            //Contract
            List<Contract> contracts = await _contractRepository.GetContractsAsync(value.Dni, accountType);
            if (contracts.Count == 0)
            {
                invitationData.Contract.State = StateEnum.Error;
                await SendEmailInvitationError(invitationData);
                throw new ServiceException("User without contract, user: " + userIdentification, HttpStatusCode.BadRequest, FieldNames.Contract, ValidationMessages.NotFound);
            }
            invitationData.Contract.SetValueAndState(contracts.Count.ToString(), StateEnum.Checked);
            invitationData.ActiveContract = UserInvitationUtils.GetMandatoryData(SystemTypes.SM, EntityNames.WBSGetContract, null, StateEnum.Unchecked);

            foreach (Contract contract in contracts)
            {
                if (contract != null && contract.Unit != null && !string.IsNullOrEmpty(contract.SmContractCode) && invitationData.ActiveContract.State == StateEnum.Unchecked)
                {
                    invitationData.SmContractCode.SetValueAndState(contract.SmContractCode, StateEnum.Checked);

                    SMContract contractSM = await _contractSMRepository.GetAccessCodeAsync(contract.SmContractCode);
                    invitationData.SMContract.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
                    if (contractSM != null && !string.IsNullOrEmpty(contractSM.Contractnumber))
                        invitationData.SMContract.SetValueAndState(contractSM.Contractnumber, StateEnum.Checked);

                    //Leaving
                   // if (!string.IsNullOrEmpty(contractSM.Leaving))
                   //     invitationData.Leaving.SetValueAndState(contractSM.Leaving.ToString(), StateEnum.Error);

                    // only active contracts, if the contract has "terminated", the field "Leaving" have information.
                    if (contractSM != null) //&& string.IsNullOrEmpty(contractSM.Leaving))
                    {
                        invitationData.ActiveContract.SetValueAndState(StateEnum.Checked.ToString(), StateEnum.Checked);

                        // Store
                        invitationData.ContractStoreCode.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
                        if (contract.StoreData.StoreCode != null)
                        {
                            invitationData.ContractStoreCode.SetValueAndState(contract.StoreData.StoreCode, StateEnum.Checked);

                            // TODO: get sizeCode form Contract or Unit
                            UnitLocationSearchFilter filter = new UnitLocationSearchFilter()
                            {
                                SiteCode = contract.StoreData.StoreCode,
                                SizeCode = contract.Unit.UnitCategory
                            };
                            List<UnitLocation> unitLocation = _unitLocationRepository.Find(filter);

                            if (contact.Language == "French")
                            {
                                invitationData.UnitPassword.SetValueAndState(ValidationMessages.NoInformationAvailable_FR, StateEnum.Warning);
                            }
                            else
                            {
                                invitationData.UnitPassword.SetValueAndState(ValidationMessages.NoInformationAvailable, StateEnum.Warning);
                            }

                            invitationData.UnitSizeCode.SetValueAndState(ValidationMessages.NoInformationAvailable, StateEnum.Warning);
                            if (unitLocation.Count > 0 && !string.IsNullOrEmpty(unitLocation[0].Description))
                                invitationData.UnitSizeCode.SetValueAndState(unitLocation[0].Description, StateEnum.Checked);

                            Store store = await _storeRepository.GetStoreAsync(contract.StoreData.StoreCode);
                            invitationData.StoreCode.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
                            if (store != null)
                            {
                                if (!string.IsNullOrEmpty(store.StoreCode))
                                    invitationData.StoreCode.SetValueAndState(store.StoreCode.ToString(), StateEnum.Checked);

                                invitationData.OpeningDaysFirst.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
                                if (!string.IsNullOrEmpty(store.OpeningDaysFirst))
                                    invitationData.OpeningDaysFirst.SetValueAndState(store.OpeningDaysFirst, StateEnum.Checked);

                                invitationData.OpeningDaysLast.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
                                if (!string.IsNullOrEmpty(store.OpeningDaysLast))
                                    invitationData.OpeningDaysLast.SetValueAndState(store.OpeningDaysLast, StateEnum.Checked);

                                invitationData.OpeningHoursFrom.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
                                if (!string.IsNullOrEmpty(store.OpeningHoursFrom))
                                    invitationData.OpeningHoursFrom.SetValueAndState(store.OpeningHoursFrom, StateEnum.Checked);

                                invitationData.OpeningHoursTo.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
                                if (!string.IsNullOrEmpty(store.OpeningHoursTo))
                                    invitationData.OpeningHoursTo.SetValueAndState(store.OpeningHoursTo, StateEnum.Checked);

                                invitationData.StoreName.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
                                if (!string.IsNullOrEmpty(store.StoreName))
                                    invitationData.StoreName.SetValueAndState(store.StoreName, StateEnum.Checked);

                                invitationData.StoreEmail.SetValueAndState(ValidationMessages.Required, StateEnum.Error);

                                string email = store.EmailAddress1 ?? store.EmailAddress2;
                                if (!string.IsNullOrEmpty(email))
                                    invitationData.StoreEmail.SetValueAndState(email, StateEnum.Checked);

                                invitationData.StoreCity.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
                                if (!string.IsNullOrEmpty(store.City))
                                    invitationData.StoreCity.SetValueAndState(store.City, StateEnum.Checked);

                                // MailType for WelcomeEmail                                
                                if (!string.IsNullOrEmpty(store.MailType))
                                    invitationData.SiteMailType.SetValueAndState(store.MailType, StateEnum.Checked);
                            }

                            //List<UnitLocation> unitLocation = _unitLocationRepository.Find(filter);
                            invitationData.UnitSizeCode.SetValueAndState(ValidationMessages.NoInformationAvailable, StateEnum.Warning);
                            if (unitLocation.Count > 0 && !string.IsNullOrEmpty(unitLocation[0].Description))
                                invitationData.UnitSizeCode.SetValueAndState(unitLocation[0].Description, StateEnum.Checked);
                        }

                        // Unit

                        //Access Code eliminado temporalmente de Mandatory Data
                        if (contact.Language == "French")
                        {
                            invitationData.UnitPassword.SetValueAndState(ValidationMessages.NoInformationAvailable_FR, StateEnum.Warning);
                        }
                        else
                        {
                            invitationData.UnitPassword.SetValueAndState(ValidationMessages.NoInformationAvailable, StateEnum.Warning);
                        }

                        if (!string.IsNullOrEmpty(contractSM.Password))
                            invitationData.UnitPassword.SetValueAndState(contractSM.Password, StateEnum.Checked);

                        if (contract.Unit != null)
                        {
                            invitationData.UnitName.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
                            var rx = new Regex(@"[a-zA-Z/,.+?-]", RegexOptions.Compiled);
                            var matchesCount = rx.Matches(contract.Unit.UnitName).Count;

                            if (matchesCount > 0)
                            {
                                if (contract.Unit.UnitName.Trim().ToUpper().StartsWith('E')) // Algunos units pueden comenzar por E (anexos a otros edificios)                                    
                                    invitationData.UnitName.SetValueAndState(contract.Unit.UnitName, StateEnum.Checked);
                                else
                                    invitationData.UnitName.SetValueAndState(string.Concat(ValidationMessages.IncorrectFormat, ": ", contract.Unit.UnitName), StateEnum.Error);
                            }
                            else if (!string.IsNullOrEmpty(contract.Unit.UnitName))
                            {
                                invitationData.UnitName.SetValueAndState(contract.Unit.UnitName, StateEnum.Checked);
                            }

                            var intSiteMailType = (int)StoreMailTypes.WithoutSignageOrNull;
                            if (!string.IsNullOrEmpty(invitationData.SiteMailType.Value))
                                intSiteMailType = Convert.ToInt32(invitationData.SiteMailType.Value.Trim());

                            switch (intSiteMailType)
                            {
                                case (int)StoreMailTypes.NewSignage:
                                    NewSignage(invitationData, contract);
                                    break;

                                case (int)StoreMailTypes.OldSignage:
                                    OldSignage(invitationData, contract);
                                    break;

                                case (int)StoreMailTypes.WithoutSignageOrNull:
                                default:
                                    WithoutSignage(invitationData);
                                    break;
                            }
                        }

                        // OpportunityCRM
                        invitationData.ContractOpportunity.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
                        if (!string.IsNullOrEmpty(contract.OpportunityId))
                        {
                            invitationData.ContractOpportunity.SetValueAndState(StateEnum.Checked.ToString(), StateEnum.Checked);

                            OpportunityCRM opportunity = await _opportunityRepository.GetOpportunity(contract.OpportunityId);

                            if (opportunity != null)
                            {
                                invitationData.OpportunityId.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
                                if (!string.IsNullOrEmpty(opportunity.OpportunityId))
                                    invitationData.OpportunityId.SetValueAndState(opportunity.OpportunityId, StateEnum.Checked);

                                invitationData.ExpectedMoveIn.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
                                if (!string.IsNullOrEmpty(opportunity.ExpectedMoveIn))
                                {
                                    DateTime moveIn = DateTime.Parse(opportunity.ExpectedMoveIn);
                                    invitationData.ExpectedMoveIn.SetValueAndState(moveIn.ToString(), StateEnum.Checked);
                                }
                            }

                        }
                    }
                }

            }

            if (invitationData.SmContractCode.State == StateEnum.Unchecked)
                invitationData.SmContractCode.SetValueAndState(ValidationMessages.Required, StateEnum.Error);

            return true;
        }

        private static void NewSignage(InvitationMandatoryData invitationData, Contract contract)
        {
            invitationData.UnitColour.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
            if (!string.IsNullOrEmpty(contract.Unit.Colour))
                invitationData.UnitColour.SetValueAndState(contract.Unit.Colour, StateEnum.Checked);

            invitationData.UnitCorridor.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
            if (!string.IsNullOrEmpty(contract.Unit.Corridor))
                invitationData.UnitCorridor.SetValueAndState(contract.Unit.Corridor, StateEnum.Checked);

            invitationData.UnitExceptions.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
            if (!string.IsNullOrEmpty(contract.Unit.Exceptions))
                invitationData.UnitExceptions.SetValueAndState(contract.Unit.Exceptions, StateEnum.Checked);

            invitationData.UnitFloor.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
            if (!string.IsNullOrEmpty(contract.Unit.Floor))
                invitationData.UnitFloor.SetValueAndState(contract.Unit.Floor, StateEnum.Checked);

            invitationData.UnitZone.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
            if (!string.IsNullOrEmpty(contract.Unit.Zone))
                invitationData.UnitZone.SetValueAndState(contract.Unit.Zone, StateEnum.Checked);
        }

        private static void OldSignage(InvitationMandatoryData invitationData, Contract contract)
        {
            invitationData.UnitExceptions.SetValueAndState(ValidationMessages.Required, StateEnum.Error);
            if (!string.IsNullOrEmpty(contract.Unit.Exceptions))
                invitationData.UnitExceptions.SetValueAndState(contract.Unit.Exceptions, StateEnum.Checked);
        }

        private static void WithoutSignage(InvitationMandatoryData invitationData)
        {
            invitationData.UnitColour.SetValueAndState(string.Empty, StateEnum.Unchecked);
            invitationData.UnitCorridor.SetValueAndState(string.Empty, StateEnum.Unchecked);
            invitationData.UnitExceptions.SetValueAndState(string.Empty, StateEnum.Unchecked);
            invitationData.UnitFloor.SetValueAndState(string.Empty, StateEnum.Unchecked);
            invitationData.UnitZone.SetValueAndState(string.Empty, StateEnum.Unchecked);
        }

        private async Task<bool> CheckMandatoryData(InvitationMandatoryData fields)
        {
            string validations = null;
            PropertyInfo[] properties = typeof(InvitationMandatoryData).GetProperties();
            foreach (var property in properties)
            {
                MandatoryData data = (MandatoryData)property.GetValue(fields);
                string value = string.Empty;
                if (data != null && (data.State != StateEnum.Checked && data.State != StateEnum.Warning && data.State != StateEnum.Unchecked))
                {
                    // Solo se tienen en cuenta como campos no válidos los que tienen estado "StateEnum.Error"
                    validations += property.Name + ", ";
                }
            }

            if (!string.IsNullOrEmpty(validations))
            {
                // Solo se envía el mail de error de campos cuando hay campos en estado "StateEnum.Error"
                await SendEmailInvitationError(fields);
                throw new ServiceException("required some fields", HttpStatusCode.BadRequest, validations, ValidationMessages.Required);
            }

            return true;
        }

        public async Task<Profile> GetUserByInvitationTokenAsync(string receivedToken)
        {
            // 1. Validate user
            if (string.IsNullOrEmpty(receivedToken)) throw new ServiceException("User must have a received Token.", HttpStatusCode.BadRequest, FieldNames.ReceivedToken, ValidationMessages.EmptyFields);

            var user = _userRepository.GetUserByInvitationToken(receivedToken);
            if (user == null)
                throw new ServiceException("User is not found.", HttpStatusCode.NotFound, FieldNames.User, ValidationMessages.NotExist);

            Profile profile = new Profile()
            {
                Fullname = user.Name,
                Language = user.Language
            };

            return await Task.FromResult(profile);
        }

        public bool ValidateEmail(string email)
        {
            User userByEmail = _userRepository.GetCurrentUserByEmail(email);
            return (userByEmail.Id == null);
        }

        public async Task<bool> SaveNewUser(NewUser newUser)
        {
            //Checks in CRM if there's an active contract for the DocumentNumber

            //AccountProfile entity = await _profileRepository.GetContactByMail(newUser.Email);
            //bool result = false;
            bool isNewUser = false;
            //DateTime date = DateTime.Now;
            //newUser.Day = date;
            //if (entity != null)
            //{


            //List<Contract> contracts = await _contractRepository.GetContractsAsync(entity.DocumentNumber, entity.CustomerType);

            //if (contracts != null)
            //{
            isNewUser = _newUserRepository.SaveNewUser(newUser).Result;

            if (isNewUser)
            {

                string mailTo = _config["MailWP"];
                if (string.IsNullOrEmpty(mailTo))
                    throw new ServiceException("Store mail not found", HttpStatusCode.NotFound, FieldNames.Email, ValidationMessages.NotFound);

                Email message = new Email();
                message.To.Add(mailTo);
                if (_config["Environment"] == nameof(EnvironmentTypes.DEV) || _config["Environment"] == nameof(EnvironmentTypes.PRE))
                {
                    message.Cc.Add(_config["MailIT"]);
                }
                message.Subject = "Solicitud nuevo usuario web portal";
                message.Body = "Nueva petición de usuario Web Portal a dia: " + DateTime.Now +
                "<br><strong>Nombre</strong>: " + newUser.Name + ", con <strong>Email</strong>: " + newUser.Email + " y <strong>Teléfono de contacto</strong>: " + newUser.Phone;

                await _mailRepository.Send(message);

            }

            //result = true;
            //    }
            //}

            return isNewUser;
        }

        public async Task<bool> ValidateCaptcha(string token)
        {
            return await _googleCaptchaRepository.IsTokenValid(token);

        }

    }
}