using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
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

        public bool CheckFeatureByNameAndEnvironment(string name, string environment)
        {
            Feature feature = new Feature();

            var features = _features.FindOne(t => t.Name == name);
            foreach (var f in features)
            {
                var env = f.Environments.Find(e => e.Name == environment);
                if (env != null && !string.IsNullOrEmpty(env.Name))
                    return env.Value;
            }

            return false;
        }
    }
}
