using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace customerportalapi.Services
{
    public class StoreImageServices : IStoreImageServices
    {
        private readonly IStoreImageRepository _storeImageRepository;
        public StoreImageServices(IStoreImageRepository storeImageRepository)
        {
            _storeImageRepository = storeImageRepository;
        }

        public StoreImage GetStoreImage(string storeCode)
        {
            StoreImage storeImage = _storeImageRepository.Get(storeCode);
            return storeImage;
        }

        public Task<bool> CreateStoreImage(StoreImage storeImage)
        {
            if (string.IsNullOrEmpty(storeImage.StoreCode))
                throw new ServiceException("StoreImage StoreCode required", HttpStatusCode.BadRequest, "StoreCode", "StoreImage StoreCode required");

            if (string.IsNullOrEmpty(storeImage.ContainerId))
                throw new ServiceException("StoreImage ContainerId required", HttpStatusCode.BadRequest, "ContainerId", "StoreImage ContainerId required");           

            StoreImage findStoreImage = _storeImageRepository.Get(storeImage.StoreCode);

            if (findStoreImage.Id != null)
                throw new ServiceException("StoreImage exist with same StoreCode please update", HttpStatusCode.BadRequest, "StoreCode", "StoreImage exist with same StoreCode please update");

            return _storeImageRepository.Create(storeImage);
        }

        public StoreImage UpdateStoreImage(StoreImage storeImage)
        {
            StoreImage findStoreImage = _storeImageRepository.Get(storeImage.StoreCode);

            if (findStoreImage == null)
                throw new ServiceException("StoreImage by StoreCode Not Found", HttpStatusCode.NotFound, "Id", "StoreImage Code Not Found");

            var storeImageToUpdate = new StoreImage()
            {
                Id = findStoreImage.Id,
                StoreCode = storeImage.StoreCode,
                ContainerId = storeImage.ContainerId
            };

            return _storeImageRepository.Update(storeImageToUpdate);
        }

        public Task<bool> DeleteStoreImage(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ServiceException("Id required", HttpStatusCode.BadRequest, "Id", "StoreImage Id required");

            return _storeImageRepository.Delete(id);
        }

        public Task<bool> DeleteStoreImageByStoreCode(string storeCode)
        {
            if (string.IsNullOrEmpty(storeCode))
                throw new ServiceException("StoreCode required", HttpStatusCode.BadRequest, "StoreCode", "StoreImage Code required");
            return _storeImageRepository.DeleteByStoreCode(storeCode);
        }

        public List<StoreImage> FindStoreImage(StoreImageSearchFilter storeImageSearchFilter)
        {
            return _storeImageRepository.Find(storeImageSearchFilter);
        }
    }
}