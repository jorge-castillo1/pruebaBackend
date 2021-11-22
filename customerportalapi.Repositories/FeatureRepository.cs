using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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
            string currentCountry =
                !String.IsNullOrWhiteSpace(countryCustomer) && countryCustomer.Length >= 5
                ? countryCustomer.Substring(0, 2)
                : null;

            if (currentCountry != null)
            {
                var env = features.Environments.Find(e => e.Name == environment);

                if (env != null && !string.IsNullOrEmpty(env.Name) && env.Value)
                {
                    if (features.CountryAvailable != null)
                    {
                        if (features.CountryAvailable.ConvertAll(x => x.ToLower().Trim()).Contains(currentCountry.ToLower().Trim()))
                            result = true;
                    }
                    else
                    {
                        // Si no está informado el array de paises, está disponible para todos los paises
                        result = true;
                    }
                }
            }

            return result;
        }
    }
}
