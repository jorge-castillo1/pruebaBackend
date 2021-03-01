using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

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

        public async Task<List<Invoice>> GetInvoicesAsync(string contractId)
        {
            var httpClient = _clientFactory.CreateClient("httpClient");
            httpClient.BaseAddress = new Uri(_configuration["GatewaySmUrl"] + _configuration["InvoiceSMAPI"]);

            var response = await httpClient.GetAsync(contractId, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new List<Invoice>();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<List<Invoice>>(result.GetValue("result").ToString());
        }

        public async Task<List<Invoice>> GetInvoicesByCustomerIdAsync(string cutomerId)
        {
            var httpClient = _clientFactory.CreateClient("httpClient");
            httpClient.BaseAddress = new Uri(_configuration["GatewaySmUrl"] + _configuration["InvoiceByCustomerIdSMAPI"] + cutomerId);

            var response = await httpClient.GetAsync(cutomerId, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new List<Invoice>();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<List<Invoice>>(result.GetValue("result").ToString());
        }

        public async Task<bool> MakePayment(MakePayment makePayment)
        {
            var httpClient = _clientFactory.CreateClient("httpClient");
            var url = new Uri(_configuration["GatewaySmUrl"] + _configuration["InvoicePaymentSMAPI"]);

            var postContent = new StringContent(JsonConvert.SerializeObject(makePayment), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, postContent);
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }

        public async Task<SubContract> GetSubContractAsync(string contractId, string unitId)
        {
            var httpClient = _clientFactory.CreateClient("httpClient");
            var url = new Uri(_configuration["GatewaySmUrl"] + _configuration["ContractSMAPI"] + contractId + "/" + unitId);

            var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new SubContract();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<SubContract>(result.GetValue("result").ToString());
        }

        public async Task<bool> UpdateAccessCodeAsync(UpdateAccessCode updateAccessCode)
        {
            var httpClient = _clientFactory.CreateClient("httpClient");
            var url = new Uri(_configuration["GatewaySmUrl"] + _configuration["ContractSMAPI"] + "access-code");
            var putContent = new StringContent(JsonConvert.SerializeObject(updateAccessCode), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync(url, putContent);
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }

        public async Task<ApsData> GetAps(ApsRequest request)
        {
            var httpClient = _clientFactory.CreateClient("httpClient");
            var url = new Uri(_configuration["GatewaySmUrl"]+ _configuration["ContractSMAPI"]+ "aps");
            var postContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, postContent);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<ApsData>(result.GetValue("result").ToString());

        }
    }
}
