using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using customerportalapi.Services.interfaces;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using customerportalapi.Services.Exceptions;

namespace customerportalapi.Services
{
    public class SiteServices : ISiteServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IContractRepository  _contractRepository;

        public SiteServices(IUserRepository userRepository, IContractRepository contractRepository)
        {
            _userRepository = userRepository;
            _contractRepository = contractRepository;
        }


        public async Task<List<Site>> GetContractsAsync(string dni)
        {
            //Add customer portal Business Logic
            User user = _userRepository.GetCurrentUser(dni);
            if (user._id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");
            
            //2. If exist complete data from external repository
            //Invoke repository
            List<Contract> entitylist = new List<Contract>();
            entitylist = await _contractRepository.GetContractsAsync(dni);

            List<Site> stores = new List<Site>();
            foreach(var storegroup in entitylist.GroupBy(x => x.Store))
            {
                Site store = new Site {Name = storegroup.Key};
                foreach (var contract in storegroup)
                    store.Contracts.Add(contract);

                stores.Add(store);
            }

            return stores;
        }
    }
}
