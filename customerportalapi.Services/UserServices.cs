﻿using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using customerportalapi.Services.interfaces;
using System;
using System.Threading.Tasks;
using customerportalapi.Entities.enums;
using Microsoft.Extensions.Configuration;
using customerportalapi.Services.Exceptions;
using System.Net;

namespace customerportalapi.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IConfiguration _config;

        public UserServices(IUserRepository userRepository, IProfileRepository profileRepository, IMailRepository mailRepository, IEmailTemplateRepository emailTemplateRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _mailRepository = mailRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _config = config;
        }


        public async Task<Profile> GetProfileAsync(string dni)
        {
            //Add customer portal Business Logic
            User user = _userRepository.GetCurrentUser(dni);
            if (user._id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //1. If emailverified is false throw error
            if (!user.emailverified)
                throw new ServiceException("User is deactivated,", HttpStatusCode.NotFound, "User", "Deactivated");

            //2. If exist complete data from external repository
            //Invoke repository
            Profile entity = new Profile();
            entity = await _profileRepository.GetProfileAsync(dni);

            //3. Set Email Principal according to external data. No two principal emails allowed
            entity.EmailAddress1Principal = false;
            entity.EmailAddress2Principal = false;

            if (entity.EmailAddress1 == user.email)
                entity.EmailAddress1Principal = true;
            else if (entity.EmailAddress2 == user.email)
                entity.EmailAddress2Principal = true;

            //4. Set Phone Principal according to external data. No two principal phones allowed
            entity.MobilePhone1Principal = false;
            entity.MobilePhonePrincipal = false;

            if (entity.MobilePhone1 == user.phone && !string.IsNullOrEmpty(user.phone))
                entity.MobilePhone1Principal = true;
            else if (entity.MobilePhone == user.phone && !string.IsNullOrEmpty(user.phone))
                entity.MobilePhonePrincipal = true;

            entity.Language = user.language;
            entity.Avatar = user.profilepicture;

            return entity;
        }

        public async Task<Profile> UpdateProfileAsync(Profile profile)
        {
            //Add customer portal Business Logic
            User user = _userRepository.GetCurrentUser(profile.DocumentNumber);
            if (user._id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //1. If emailverified is false throw error
            if (!user.emailverified)
                throw new ServiceException("User is deactivated,", HttpStatusCode.NotFound, "User", "Deactivated");

            //2. Set Email Principal according to external data
            if (string.IsNullOrEmpty(profile.EmailAddress1) && string.IsNullOrEmpty(profile.EmailAddress2))
                throw new ServiceException("Email field can not be null.", HttpStatusCode.BadRequest, "Email", "Empty fields");

            if (profile.EmailAddress1Principal && string.IsNullOrEmpty(profile.EmailAddress1))
                throw new ServiceException("Principal email can not be null.", HttpStatusCode.BadRequest, "Principal email", "Empty field");
            
            if (profile.EmailAddress2Principal && string.IsNullOrEmpty(profile.EmailAddress2))
                throw new ServiceException("Principal email can not be null.", HttpStatusCode.BadRequest, "Principal email", "Empty field");

            string emailToUpdate = string.Empty;
            if (profile.EmailAddress1Principal)
                emailToUpdate = profile.EmailAddress1;
            else
                emailToUpdate = profile.EmailAddress2;

            //3. Set Phone Principal according to data
            string phoneToUpdate = string.Empty;
            if (profile.MobilePhone1Principal && !string.IsNullOrEmpty(profile.MobilePhone1))
                phoneToUpdate = profile.MobilePhone1;
            else if (profile.MobilePhonePrincipal && !string.IsNullOrEmpty(profile.MobilePhone))
                phoneToUpdate = profile.MobilePhone;

            //4. Compare language, email and image for backend changes
            if (user.language != profile.Language ||
                user.profilepicture != profile.Avatar ||
                user.email != emailToUpdate ||
                user.phone != phoneToUpdate)
            {
                user.language = profile.Language;
                user.email = emailToUpdate;
                user.phone = phoneToUpdate;
                user.profilepicture = profile.Avatar;

                user = _userRepository.Update(user);
            }

            //5. Invoke repository for other changes
            Profile entity = new Profile();
            entity = await _profileRepository.UpdateProfileAsync(profile);
            entity.Language = user.language;
            entity.Avatar = user.profilepicture;
            if (entity.EmailAddress1 == user.email)
                entity.EmailAddress1Principal = true;
            else
                entity.EmailAddress2Principal = true;

            if (entity.MobilePhone1 == user.phone && !string.IsNullOrEmpty(user.phone))
                entity.MobilePhone1Principal = true;
            else if (entity.MobilePhone == user.phone && !string.IsNullOrEmpty(user.phone))
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
            User user = _userRepository.GetCurrentUser(value.Dni);
            if (user._id == null)
            {
                //4. TODO Create user in autentication system

                //5. Create user in portal database
                User newUser = new User();
                newUser.dni = value.Dni;
                newUser.email = value.Email;
                newUser.language = InvitationUtils.GetLanguage(value.Language);
                newUser.usertype = InvitationUtils.GetUserType(value.CustomerType);
                newUser.emailverified = false;
                newUser.invitationtoken = Guid.NewGuid().ToString();

                result = await _userRepository.Create(newUser);
            }
            else
            {
                //6. If emailverified is false resend email invitation otherwise throw error
                if (user.emailverified)
                    throw new ServiceException("Invitation user fails. User was actived before", HttpStatusCode.NotFound, "User", "Already invited");

                //7. Update invitation data
                user.email = value.Email;
                user.language = InvitationUtils.GetLanguage(value.Language);
                user.usertype = InvitationUtils.GetUserType(value.CustomerType);
                user.invitationtoken = Guid.NewGuid().ToString();
                _userRepository.Update(user);
            }

            //4. Get Email Invitation Template
            EmailTemplate invitationTemplate = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.Invitation, user.language);
            if (invitationTemplate._id == null)
            {
                invitationTemplate = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.Invitation, LanguageTypes.en.ToString());
            } 

            if (invitationTemplate._id != null)
            {
                //5. Sens email invitation
                Email message = new Email();
                message.To.Add(user.email);
                message.Subject = invitationTemplate.subject;
                message.Body = string.Format(invitationTemplate.body, value.Fullname, value.Dni, value.Dni,
                    $"{_config["InviteConfirmation"]}{user.invitationtoken}");
                result = await _mailRepository.Send(message);
            }

            return result;
        }

        public Task<bool> ConfirmUserAsync(string invitationToken)
        {
            //1. Validate invitationToken not empty
            if (string.IsNullOrEmpty(invitationToken))
                throw new ServiceException("User must have a invitationToken.", HttpStatusCode.BadRequest, "Invitation Token", "Empty field");

            //2. Validate user by invitationToken
            User user = _userRepository.GetUserByInvitationToken(invitationToken);
            if (user._id == null)
                return Task.FromResult(false);

            //3. Update email verification data
            user.emailverified = true;
            user.invitationtoken = null;
            _userRepository.Update(user);

            return Task.FromResult(true);
        }

        public Task<bool> UnInviteUserAsync(string dni)
        {
            //1. Validate dni not empty
            if (string.IsNullOrEmpty(dni))
                throw new ServiceException("User must have a valid document number.", HttpStatusCode.BadRequest, "Dni", "Empty field");

            //2. Validate user
            User user = _userRepository.GetCurrentUser(dni);
            if (user._id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //3. If emailverified is false
            if (!user.emailverified)
                return Task.FromResult(false);

            //4. Update invitation data
            user.emailverified = false;
            user.invitationtoken = null;
            _userRepository.Update(user);

            return Task.FromResult(true);
        }
    }
}
