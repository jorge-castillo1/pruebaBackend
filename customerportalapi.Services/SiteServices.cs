using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using customerportalapi.Services.interfaces;
using System;
using System.Threading.Tasks;
using customerportalapi.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace customerportalapi.Services
{
    public class SiteServices : ISiteServices
    {
        readonly IUserRepository _userRepository;
        readonly IContractRepository  _contractRepository;

        public SiteServices(IUserRepository userRepository, IContractRepository contractRepository)
        {
            _userRepository = userRepository;
            _contractRepository = contractRepository;
        }


        public async Task<List<Site>> GetContractsAsync(string dni)
        {
            //Add customer portal Business Logic
            User user = _userRepository.getCurrentUser(dni);
            if (user._id == null)
                throw new ArgumentException("User does not exist.");


            //2. If exist complete data from external repository
            //Invoke repository
            List<Contract> entitylist = new List<Contract>();
            entitylist = await _contractRepository.GetContractsAsync(dni);

            List<Site> stores = new List<Site>();
            foreach(var storegroup in entitylist.GroupBy(x => x.Store))
            {
                Site store = new Site();
                store.Name = storegroup.Key;
                foreach (var contract in storegroup)
                    store.Contracts.Add(contract);

                stores.Add(store);
            }

            return stores;
        }
    }
}
