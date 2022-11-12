using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace customerportalapi.Repositories
{
    public class FeatureRepository : IFeatureRepository
    {
        private readonly IMongoCollectionWrapper<Feature> _features;

        public FeatureRepository(IConfiguration config, IMongoCollectionWrapper<Feature> features)
        {
            _features = features;
        }

        public Task<bool> Create(Feature feature)
        {
            _features.InsertOne(feature);

            return Task.FromResult(true);
        }

        public bool CheckFeatureByNameAndEnvironment(string name, string environment, string countryCustomer)
        {
            Feature feature = new Feature();

            bool result = false;

            var features = _features.FindOne(t => t.Name == name).FirstOrDefault();

            if (!string.IsNullOrEmpty(countryCustomer) && features != null)
            {
                var env = features.Environments.Find(e => e.Name == environment);

                if (env != null && !string.IsNullOrEmpty(env.Name) && env.Value != null && !string.IsNullOrEmpty(env.Value.ToString()) && bool.TryParse(env.Value.ToString(), out bool valor))
                {
                    if (valor)
                    {
                        if (features.CountryAvailable != null && features.CountryAvailable.Count > 0)
                        {
                            if (features.CountryAvailable.ConvertAll(x => x.ToLower().Trim())
                                .Contains(countryCustomer.ToLower().Trim()))
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            // Si no está informado el array de paises, está disponible para todos los paises
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        public int CheckFeature(string name, string environment, string country = "", int defaultValue = 0)
        {
            var result = defaultValue;

            var features = _features.FindOne(t => t.Name == name).FirstOrDefault();
            if (features != null)
            {
                // Si el array de paises está relleno y no viene informado el país en cuestión, que devuelva el valor por defecto.
                // Si el array de paises no está informado, esta característica estará disponible para todos los paises.
                if (features.CountryAvailable != null &&
                    features.CountryAvailable.Count > 0 &&
                    features.CountryAvailable.ConvertAll(x => x.ToLower().Trim()).Contains(country.ToLower().Trim()) == false)
                {
                    return result;
                }

                // Si tiene pais, se consulta el valor por el entorno
                var env = features.Environments.Find(e => e.Name == environment);
                if (env != null &&
                    !string.IsNullOrEmpty(env.Name) &&
                    env.Value != null &&
                    !string.IsNullOrEmpty(env.Value.ToString()) &&
                    int.TryParse(env.Value.ToString(), out var valor))
                {
                    if (valor > 0)
                    {
                        result = valor;
                    }
                }
            }

            return result;
        }

        public string CheckFeature(string name, string environment, string country = "", string defaultValue = "")
        {
            var result = defaultValue;

            var features = _features.FindOne(t => t.Name == name).FirstOrDefault();
            if (features != null)
            {
                // Si el array de paises está relleno y no viene informado el país en cuestión, que devuelva el valor por defecto.
                // Si el array de paises no está informado, esta característica estará disponible para todos los paises.
                if (!string.IsNullOrEmpty(country) &&
                    features.CountryAvailable != null &&
                    features.CountryAvailable.Count > 0 &&
                    features.CountryAvailable.ConvertAll(x => x.ToLower().Trim()).Contains(country.ToLower().Trim()) == false)
                {
                    return result;
                }

                // Si tiene pais, se consulta el valor por el entorno
                var env = features.Environments.Find(e => e.Name == environment);
                if (env != null &&
                    !string.IsNullOrEmpty(env.Name) &&
                    env.Value != null &&
                    !string.IsNullOrEmpty(env.Value.ToString()))
                {
                    result = env.Value.ToString();
                }
            }

            return result;
        }
    }
}
