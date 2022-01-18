using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace customerportalapi.Repositories
{
    public class AccountSMRepository : IAccountSMRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public AccountSMRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<bool> AddBankAccountAsync(SMBankAccount account)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            var url = new Uri(_configuration["GatewaySmUrl"] + _configuration["SMAccountAPI"]);

            var postContent = new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, postContent);
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }
    }
}
