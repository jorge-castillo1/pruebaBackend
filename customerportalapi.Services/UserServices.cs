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

            //TODO: Validate Principal Email
            string emailToUpdate = string.Empty;
            if (!string.IsNullOrEmpty(profile.EmailAddress1))
                emailToUpdate = profile.EmailAddress1;
            else
            {
                if (!string.IsNullOrEmpty(profile.EmailAddress2))
                    emailToUpdate = profile.EmailAddress2;
            }

            //1. Compare language, email and image for backend changes
            if (user.language.ToLower() != profile.Language.ToLower() ||
                user.profilepicture.ToLower() != profile.Avatar.ToLower() ||
                user.email.ToLower() != emailToUpdate.ToLower())
            {
                if (user.language.ToLower() != profile.Language.ToLower())
                    user.language = profile.Language;

                if (user.email.ToLower() != emailToUpdate.ToLower() && !string.IsNullOrEmpty(emailToUpdate))
                    user.email = profile.EmailAddress1;

                if (user.profilepicture.ToLower() != profile.Avatar.ToLower())
                    user.profilepicture = profile.Avatar;

                user = _userRepository.update(user);
            }

            //2. Invoke repository for other changes
            Profile entity = new Profile();
            entity = await _profileRepository.UpdateProfileAsync(profile);
            entity.Language = user.language;
            entity.Avatar = user.profilepicture;

            return entity;
        }
    }
}
