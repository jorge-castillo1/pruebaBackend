using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using customerportalapi.Services.interfaces;
using System;
using System.Threading.Tasks;
using customerportalapi.Repositories;

namespace customerportalapi.Services
{
    public class UserServices : IUserServices
    {
        readonly IUserRepository _userRepository;
        readonly IProfileRepository  _profileRepository;

        public UserServices(IUserRepository userRepository, IProfileRepository profileRepository)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
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
            else 
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
    }
}
