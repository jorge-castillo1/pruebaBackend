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

        public async Task<bool> MakePayment(MakePayment makePayment)
        {
            var httpClient = _clientFactory.CreateClient("httpClient");
            var url = new Uri(_configuration["GatewaySmUrl"] + _configuration["InvoicePaymentSMAPI"]);

            var postContent = new StringContent(JsonConvert.SerializeObject(makePayment), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, postContent);
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }
    }
}
