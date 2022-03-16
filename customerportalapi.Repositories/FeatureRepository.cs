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

            if (!string.IsNullOrEmpty(countryCustomer) && features!=null)
            {
                var env = features.Environments.Find(e => e.Name == environment);

                if (env != null && !string.IsNullOrEmpty(env.Name) && env.Value)
                {
                    if (features.CountryAvailable != null && features.CountryAvailable.Count>0)
                    {
                        if (features.CountryAvailable.ConvertAll(x => x.ToLower().Trim()).Contains(countryCustomer.ToLower().Trim()))
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

            return result;
        }
    }
}
