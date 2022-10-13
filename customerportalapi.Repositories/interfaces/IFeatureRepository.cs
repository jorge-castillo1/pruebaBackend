using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IFeatureRepository
    {
        Task<bool> Create(Feature feature);
        bool CheckFeatureByNameAndEnvironment(string name, string environment, string countryCustomer);
        int CheckFeature(string name, string environment, string country = "", int defaultValue = 0);
        string CheckFeature(string name, string environment, string country = "", string defaultValue = "");
    }
}
