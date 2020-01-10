using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using customerportalapi.Services.interfaces;
using System;
using System.Threading.Tasks;
using customerportalapi.Repositories;
using System.Collections.Generic;
using System.Net.Mail;

namespace customerportalapi.Services
{
    public class UserServices : IUserServices
    {
        readonly IUserRepository _userRepository;
        readonly IProfileRepository  _profileRepository;
        readonly IMailRepository _mailRepository;

        public UserServices(IUserRepository userRepository, IProfileRepository profileRepository, IMailRepository mailRepository)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _mailRepository = mailRepository;
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

            //4. Sens email invitation
            Email message = new Email();
            message.To.Add(user.email);
            message.Subject = InvitationUtils.GetWelcomeMessage(value.Language);

            string logoimage = "data:image/jpeg;base64,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";
            message.Body = String.Format("<html><body><img src='{0}'/><p>Hola {1},</p><p>Bienvenido a la área privada de clientes de Bluespace.</br>Pulsa el enlace adjunto a este correo para aceptar la invitación y empezar a disfrutar de las ventajas que te ofrece el portal.</p><p>Tu usario de acceso al portal es: {2}<br>Tu contraseña de acceso al portal es: {3}</p><p>Te recomendamos que cambies tu contraseña una vez entres al portal desde la sección de 'Mi Perfil'.</p><p><a href='{4}'>{5}</a></p></body>", logoimage, value.Fullname, value.Dni, value.Dni, string.Format("http://localhost:44332/users/invite/{0}", user.invitationtoken), "Aceptar Invitación");
            result = await _mailRepository.Send(message);

            return result;
        }

        public void DesInvitar()
        {
            //Establecer email verified a false para que no pueda acceder al portal
        }
    }
}
