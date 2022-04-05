using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IOpportunityCRMRepository
    {
        Task<OpportunityCRM> GetOpportunity(string opportunityId);

    }
}
