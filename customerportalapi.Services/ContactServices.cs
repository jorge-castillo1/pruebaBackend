using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using customerportalapi.Services.interfaces;
using System;
using System.Threading.Tasks;
using customerportalapi.Repositories;

namespace customerportalapi.Services
{
    public class ContactServices : IContactServices
    {
        readonly IUserRepository _userRepository;
        readonly IContactRepository  _contactRepository;

        public ContactServices(IUserRepository userRepository, IContactRepository contactRepository)
        {
            _userRepository = userRepository;
            _contactRepository = contactRepository;
        }


        public async Task<Contact> GetContactAsync(string dni)
        {
            //Add customer portal Business Logic
            User user = _userRepository.getCurrentUser(dni);
            if (user._id == null)
                throw new ArgumentException("User does not exist.");

            
            //2. If exist complete data from external repository
            //Invoke repository
            Contact entity = new Contact();
            entity = await _contactRepository.GetContactAsync(dni);
            entity.LanguageCode = user.language;
            entity.Avatar = user.profilepicture;

            return entity;
        }

        public async Task<Contact> UpdateContactAsync(Contact contact)
        {
            //Add customer portal Business Logic
            User user = _userRepository.getCurrentUser(contact.DocumentNumber);
            if (user._id == null)
                throw new ArgumentException("User does not exist.");

            //1. Compare language and image for backend changes
            if (user.language.ToLower() != contact.LanguageCode.ToLower() ||
                user.profilepicture.ToLower() != contact.Avatar.ToLower())
            {
                if (user.language.ToLower() != contact.LanguageCode.ToLower())
                    user.language = contact.LanguageCode;
                
                if (user.profilepicture.ToLower() != contact.Avatar.ToLower())
                    user.profilepicture = contact.Avatar;

                user = _userRepository.update(user);
            }

            //2. Invoke repository for other changes
            Contact entity = new Contact();
            entity = await _contactRepository.UpdateContactAsync(contact);
            entity.LanguageCode = user.language;
            entity.Avatar = user.profilepicture;

            return entity;
        }
    }
}
