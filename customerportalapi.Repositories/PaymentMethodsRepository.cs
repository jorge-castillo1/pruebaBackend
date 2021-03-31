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
    public class PaymentMethodsRepository : IPaymentMethodRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public PaymentMethodsRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<PaymentMethodCRM> GetPaymentMethod(string storeId)
        {
            var httpClient = _clientFactory.CreateClient("httpClient");
            var uri = new Uri(_configuration["GatewayUrl"] + _configuration["PaymentMethodsCRM"]);
            httpClient.BaseAddress = uri;

            var response = await httpClient.GetAsync(storeId, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new PaymentMethodCRM();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<PaymentMethodCRM>(result.GetValue("result").ToString());
        }
        public async Task<PaymentMethodCRM> GetPaymentMethodByCard(string storeId)
        {
            var httpClient = _clientFactory.CreateClient("httpClient");
            var uri = new Uri(_configuration["GatewayUrl"] + _configuration["PaymentMethodsCRM"] + storeId + "/card/");

            var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new PaymentMethodCRM();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<PaymentMethodCRM>(result.GetValue("result").ToString());
        }

        public async Task<PaymentMethodCRM> GetPaymentMethodByBankAccount(string storeId)
        {
            var httpClient = _clientFactory.CreateClient("httpClient");
            var uri = new Uri(_configuration["GatewayUrl"] + _configuration["PaymentMethodsCRM"] + storeId + "/bank/");

            var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new PaymentMethodCRM();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<PaymentMethodCRM>(result.GetValue("result").ToString());
        }

        public async Task<PaymentMethodCRM> GetPaymentMethodById(string paymentMethodId)
        {
            var httpClient = _clientFactory.CreateClient("httpClient");
            var uri = new Uri(_configuration["GatewayUrl"] + _configuration["PaymentMethodsCRM"] + "method/");
            httpClient.BaseAddress = uri;

            var response = await httpClient.GetAsync(paymentMethodId, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new PaymentMethodCRM();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<PaymentMethodCRM>(result.GetValue("result").ToString());
        }

    }
}
