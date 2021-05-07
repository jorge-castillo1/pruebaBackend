using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IFeatureRepository
    {
        Task<bool> Create(Feature feature);
        bool CheckFeatureByNameAndEnvironment(string name, string environment);
    }
}
