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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public PaymentRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<string> ChangePaymentMethodCard(HttpContent content)
        {
            string entity = "";

            var httpClient = _clientFactory.CreateClient("httpClientPayment");
            httpClient.BaseAddress = new Uri(_configuration["GatewayPaymentUrl"] + _configuration["ChangePaymentMethodCardEndpoint"]);

            var response = await httpClient.PostAsync(httpClient.BaseAddress, content);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entity;

            var value = await response.Content.ReadAsStringAsync();
            
            return value;
        }

        public async Task<PaymentMethodCardConfirmationResponse> ConfirmChangePaymentMethodCard(PaymentMethodCardConfirmationToken confirmation)
        {
            PaymentMethodCardConfirmationResponse entity = new PaymentMethodCardConfirmationResponse();

            var httpClient = _clientFactory.CreateClient("httpClientPayment");
            httpClient.BaseAddress = new Uri(_configuration["GatewayPaymentUrl"] + _configuration["CardConfirmationTokenEndpoint"]);

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("channel", "WEBPORTAL"));
            keyValues.Add(new KeyValuePair<string, string>("externalid", confirmation.ExternalId));
            keyValues.Add(new KeyValuePair<string, string>("confirmed", confirmation.Confirmed == true ? "true" : "false"));
            HttpContent content = new FormUrlEncodedContent(keyValues);
        
            var response = await httpClient.PostAsync(httpClient.BaseAddress, content);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entity;

            var value = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(value);

            return JsonConvert.DeserializeObject<PaymentMethodCardConfirmationResponse>(result.ToString());

        }

    }
}
