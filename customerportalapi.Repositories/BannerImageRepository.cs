using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace customerportalapi.Repositories
{
    public class BannerImageRepository : IBannerImageRepository
    {
        private readonly IMongoCollectionWrapper<BannerImage> _bannerImages;

        public BannerImageRepository(IConfiguration config, IMongoCollectionWrapper<BannerImage> bannerImages)
        {
            _bannerImages = bannerImages;
        }

        public string GetUrlImage(string countryCode, string userLanguage)
        {
            var bannerImage = _bannerImages.FindOne(t => t.CountryCode.ToLower() == countryCode.ToLower() &&
                                                         t.UserLanguage.ToLower() == userLanguage.ToLower() &&
                                                         t.Active).FirstOrDefault();
            return bannerImage != null ? bannerImage.ImageUrl : string.Empty;
        }
    }
}