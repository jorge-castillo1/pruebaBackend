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
            entity.LanguageCode = user.language;
            entity.Avatar = user.profilepicture;

            return entity;
        }

        public async Task<Profile> UpdateProfileAsync(Profile profile)
        {
            //Add customer portal Business Logic
            User user = _userRepository.getCurrentUser(profile.DocumentNumber);
            if (user._id == null)
                throw new ArgumentException("User does not exist.");

            //1. Compare language, email and image for backend changes
            if (user.language.ToLower() != profile.LanguageCode.ToLower() ||
                user.profilepicture.ToLower() != profile.Avatar.ToLower() ||
                user.email.ToLower() != profile.EmailAddress1.ToLower())
            {
                if (user.language.ToLower() != profile.LanguageCode.ToLower())
                    user.language = profile.LanguageCode;

                if (user.email.ToLower() != profile.EmailAddress1.ToLower())
                    user.email = profile.EmailAddress1;

                if (user.profilepicture.ToLower() != profile.Avatar.ToLower())
                    user.profilepicture = profile.Avatar;

                user = _userRepository.update(user);
            }

            //2. Invoke repository for other changes
            Profile entity = new Profile();
            entity = await _profileRepository.UpdateProfileAsync(profile);
            entity.LanguageCode = user.language;
            entity.Avatar = user.profilepicture;

            return entity;
        }
    }
}
