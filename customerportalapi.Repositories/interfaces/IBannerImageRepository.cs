namespace customerportalapi.Repositories.Interfaces
{
    public interface IBannerImageRepository
    {
        string GetUrlImage(string countryCode, string userLanguage);
    }
}
