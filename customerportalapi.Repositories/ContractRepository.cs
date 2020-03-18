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

        public async Task<Contract> GetContractAsync(string contractNumber)
        {
            var entity = new Contract();

            var httpClient = _clientFactory.CreateClient("httpClient");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ContractsAPI"]);

            var response = await httpClient.GetAsync("code/" + contractNumber, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entity;
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var contract = JsonConvert.DeserializeObject<Contract>(result.GetValue("result").ToString());

            return contract;
        }

        public async Task<string> GetDownloadContractAsync(string contractNumber)
        {
            string entity = null;

            var httpClient = _clientFactory.CreateClient("httpClientDocument");
            httpClient.BaseAddress = new Uri(_configuration["GatewayDocumentsUrl"] + _configuration["DocumentsAPI"]);

            var response = await httpClient.GetAsync("contract/" + contractNumber, HttpCompletionOption.ResponseHeadersRead);
            //response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return "";
                return entity;
            }
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var contractFile = result.GetValue("result").ToString();
            return contractFile;

        }

        public async Task<string> SaveContractAsync(Document document)
        {
            var httpClient = _clientFactory.CreateClient("httpClientDocument");
            
            var url = new Uri(httpClient.BaseAddress + _configuration["DocumentsAPI"]);
            var postContent = new StringContent(JsonConvert.SerializeObject(document), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, postContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var documentId = result.GetValue("result").ToString();
            return documentId;  
        }
    }
}
