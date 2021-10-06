using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace customerportalapi.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public ContractRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<List<Contract>> GetContractsAsync(string dni, string accountType)
        {
            var entitylist = new List<Contract>();

            var httpClient = _clientFactory.CreateClient("httpClient");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ContractsAPI"]);

            var response = await httpClient.GetAsync(dni + "/" + accountType, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entitylist;
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var contractList = JsonConvert.DeserializeObject<List<Contract>>(result.GetValue("result").ToString());

            return contractList;
        }

        public async Task<Contract> GetContractAsync(string smContractCode)
        {
            var entity = new Contract();

            var httpClient = _clientFactory.CreateClient("httpClient");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ContractsAPI"]);

            var response = await httpClient.GetAsync("code/" + smContractCode, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entity;
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var contract = JsonConvert.DeserializeObject<Contract>(result.GetValue("result").ToString());

            return contract;
        }

        public async Task<Contract> UpdateContractAsync(Contract cont)
        {
            var entity = new Contract();

            var httpClient = _clientFactory.CreateClient("httpClient");
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, new Uri(_configuration["GatewayUrl"] + _configuration["ContractsAPI"]))
            {
                Content = new StringContent(JsonConvert.SerializeObject(cont), Encoding.UTF8, "application/json")
            };
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<Contract>(result.GetValue("result").ToString());

        }


    }
}
