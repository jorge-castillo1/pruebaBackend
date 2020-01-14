﻿using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using customerportalapi.Services.interfaces;
using System;
using System.Threading.Tasks;
using customerportalapi.Repositories;
using System.Collections.Generic;
using System.Net.Mail;
using customerportalapi.Entities.enums;
using Microsoft.Extensions.Configuration;

namespace customerportalapi.Services
{
    public class UserServices : IUserServices
    {
        readonly IUserRepository _userRepository;
        readonly IProfileRepository  _profileRepository;
        readonly IMailRepository _mailRepository;
        readonly IEmailTemplateRepository _emailTemplateRepository;
        readonly IConfiguration _config;

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
            User user = _userRepository.getCurrentUser(dni);
            if (user._id == null)
                throw new ArgumentException("User does not exist.");


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
            
            entity.Language = user.language;
            entity.Avatar = user.profilepicture;

            return entity;
        }

        public async Task<Profile> UpdateProfileAsync(Profile profile)
        {
            //Add customer portal Business Logic
            User user = _userRepository.getCurrentUser(profile.DocumentNumber);
            if (user._id == null)
                throw new ArgumentException("User does not exist.");

            //3. Set Email Principal according to external data
            if (String.IsNullOrEmpty(profile.EmailAddress1) && String.IsNullOrEmpty(profile.EmailAddress2))
                throw new ArgumentException("Email field can not be null.");

            if (profile.EmailAddress1Principal && String.IsNullOrEmpty(profile.EmailAddress1))
                throw new ArgumentException("Principal email can not be null.");

            if (profile.EmailAddress2Principal && String.IsNullOrEmpty(profile.EmailAddress2))
                throw new ArgumentException("Principal email can not be null.");

            string emailToUpdate = string.Empty;
            if (profile.EmailAddress1Principal)
                emailToUpdate = profile.EmailAddress1;
            else
                emailToUpdate = profile.EmailAddress2;
            
            //1. Compare language, email and image for backend changes
            if (user.language != profile.Language ||
                user.profilepicture != profile.Avatar ||
                user.email != emailToUpdate)
            {
                user.language = profile.Language;
                user.email = emailToUpdate;
                user.profilepicture = profile.Avatar;

                user = _userRepository.update(user);
            }

            //2. Invoke repository for other changes
            Profile entity = new Profile();
            entity = await _profileRepository.UpdateProfileAsync(profile);
            entity.Language = user.language;
            entity.Avatar = user.profilepicture;
            if (entity.EmailAddress1 == user.email)
                entity.EmailAddress1Principal = true;
            else
                entity.EmailAddress2Principal = true;

            return entity;
        }

        public async Task<bool> InviteUserAsync(Invitation value)
        {
            bool result = false;

            //1. Validate email not empty
            if (string.IsNullOrEmpty(value.Email))
                throw new ArgumentException("User must have a valid email address.");
            //2. Validate dni not empty
            if (string.IsNullOrEmpty(value.Dni))
                throw new ArgumentException("User must have a valid document number.");

            //3. If no user exists create user
            User user = _userRepository.getCurrentUser(value.Dni);
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

                result = await _userRepository.create(newUser);
            }
            else
            {
                //6. If emailverified is false resend email invitation otherwise throw error
                if (user.emailverified)
                    throw new InvalidOperationException("Invitation user fails. User already exist");

                //7. Update invitation data
                user.email = value.Email;
                user.language = InvitationUtils.GetLanguage(value.Language);
                user.usertype = InvitationUtils.GetUserType(value.CustomerType);
                user.invitationtoken = Guid.NewGuid().ToString();
                _userRepository.update(user);
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
                message.Body = String.Format(invitationTemplate.body, value.Fullname, value.Dni, value.Dni, string.Format("{0}{1}", _config["InviteConfirmation"], user.invitationtoken));
                result = await _mailRepository.Send(message);
            }

            return result;
        }

        public void DesInvitar()
        {
            //Establecer email verified a false para que no pueda acceder al portal
        }
    }
}
