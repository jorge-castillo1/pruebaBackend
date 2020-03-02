using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace customerportalapi.Repositories
{
    public class ContractSMRepository : IContractSMRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public ContractSMRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }
        
        public async Task<SMContract> GetAccessCodeAsync(string contractId)
        {
            var httpClient = _clientFactory.CreateClient("httpClient");
            httpClient.BaseAddress = new Uri(_configuration["GatewaySmUrl"] + _configuration["ContractSMAPI"]);

            var response = await httpClient.GetAsync(contractId, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new SMContract();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<SMContract>(result.GetValue("result").ToString());
        }
    }
}
