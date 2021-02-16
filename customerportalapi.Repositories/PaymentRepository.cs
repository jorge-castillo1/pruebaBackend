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
using Microsoft.Extensions.Logging;

namespace customerportalapi.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        private readonly ILogger<PaymentRepository> _logger;


        public PaymentRepository(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<PaymentRepository> logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
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

        public async Task<PaymentMethodCardConfirmationResponse> UpdateConfirmChangePaymentMethodCard(PaymentMethodCardConfirmationToken confirmation)
        {
            PaymentMethodCardConfirmationResponse entity = new PaymentMethodCardConfirmationResponse();

            var httpClient = _clientFactory.CreateClient("httpClientPayment");
            httpClient.BaseAddress = new Uri(_configuration["GatewayPaymentUrl"] + _configuration["UpdateCardConfirmationTokenEndpoint"]);

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

        public async Task<PaymentMethodGetCardResponse> GetCard(string token)
        {
            PaymentMethodGetCardResponse entity = new PaymentMethodGetCardResponse();

            var httpClient = _clientFactory.CreateClient("httpClientPayment");
            httpClient.BaseAddress = new Uri(_configuration["GatewayPaymentUrl"] + _configuration["GetCardEndpoint"]);


            var response = await httpClient.GetAsync("?token="+ token + "&channel=WEBPORTAL", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entity;

            var value = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(value);

            return JsonConvert.DeserializeObject<PaymentMethodGetCardResponse>(result.ToString());

        }

        public async Task<PaymentMethodPayInvoiceResponse> PayInvoice(PaymentMethodPayInvoice payInvoice)
        {
            PaymentMethodPayInvoiceResponse entity = new PaymentMethodPayInvoiceResponse();

            var httpClient = _clientFactory.CreateClient("httpClientPayment");
            httpClient.BaseAddress = new Uri(_configuration["GatewayPaymentUrl"] + _configuration["PayInvoice"]);

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("externalid", Guid.NewGuid().ToString()));
            keyValues.Add(new KeyValuePair<string, string>("channel", "WEBPORTAL"));
            keyValues.Add(new KeyValuePair<string, string>("siteid", payInvoice.SiteId));
            keyValues.Add(new KeyValuePair<string, string>("idcustomer", payInvoice.IdCustomer));
            keyValues.Add(new KeyValuePair<string, string>("token", payInvoice.Token));
            keyValues.Add(new KeyValuePair<string, string>("amount", payInvoice.Amount.ToString().Replace(",", ".")));
            keyValues.Add(new KeyValuePair<string, string>("ourref", payInvoice.Ourref));
            keyValues.Add(new KeyValuePair<string, string>("documentid", payInvoice.Ourref));
            HttpContent content = new FormUrlEncodedContent(keyValues);

            _logger.LogInformation("PaymentRepositoryPayInvoice:" + Guid.NewGuid().ToString() + "||" + payInvoice.Amount.ToString().Replace(",", ".") +"||" );

            var response = await httpClient.PostAsync(httpClient.BaseAddress, content);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entity;

            var value = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(value);

            return JsonConvert.DeserializeObject<PaymentMethodPayInvoiceResponse>(result.ToString());

        }

        public async Task<string> PayInvoiceNewCard(PaymentMethodPayInvoiceNewCard payInvoiceNewCard)
        {
            string entity = "";

            var httpClient = _clientFactory.CreateClient("httpClientPayment");
            httpClient.BaseAddress = new Uri(_configuration["GatewayPaymentUrl"] + _configuration["PayInvoiceNewCard"]);

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("recurrent", payInvoiceNewCard.Recurrent == true ?  "true" : "false"));
            keyValues.Add(new KeyValuePair<string, string>("externalid", payInvoiceNewCard.ExternalId));
            keyValues.Add(new KeyValuePair<string, string>("channel", "WEBPORTAL"));
            keyValues.Add(new KeyValuePair<string, string>("siteid", payInvoiceNewCard.SiteId));
            keyValues.Add(new KeyValuePair<string, string>("idcustomer", payInvoiceNewCard.IdCustomer));
            keyValues.Add(new KeyValuePair<string, string>("nif", payInvoiceNewCard.Nif));
            keyValues.Add(new KeyValuePair<string, string>("name", payInvoiceNewCard.Name));
            keyValues.Add(new KeyValuePair<string, string>("surnames", payInvoiceNewCard.Surnames));
            keyValues.Add(new KeyValuePair<string, string>("url", payInvoiceNewCard.Url));
            keyValues.Add(new KeyValuePair<string, string>("amount", payInvoiceNewCard.Amount.ToString().Replace(",", ".")));
            keyValues.Add(new KeyValuePair<string, string>("ourref", payInvoiceNewCard.Ourref));
            keyValues.Add(new KeyValuePair<string, string>("documentid", payInvoiceNewCard.DocumentId));
            keyValues.Add(new KeyValuePair<string, string>("language", payInvoiceNewCard.Language));
            HttpContent content = new FormUrlEncodedContent(keyValues);

            var response = await httpClient.PostAsync(httpClient.BaseAddress, content);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entity;

            var value = await response.Content.ReadAsStringAsync();

            return value;
        }

        public async Task<string> UpdateCardLoad(PaymentMethodUpdateCardData updateCardData)
        {
            string entity = "";

            string phoneNumber = updateCardData.PhonePrefix + "|" + updateCardData.PhoneNumber;
            var httpClient = _clientFactory.CreateClient("httpClientPayment");
            httpClient.BaseAddress = new Uri(_configuration["GatewayPaymentUrl"] + _configuration["UpdateCardEndpoint"]);
             var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("externalid", updateCardData.ExternalId));
            keyValues.Add(new KeyValuePair<string, string>("channel", "WEBPORTAL"));
            keyValues.Add(new KeyValuePair<string, string>("siteid", updateCardData.SiteId));
            keyValues.Add(new KeyValuePair<string, string>("idcustomer", updateCardData.IdCustomer));
            keyValues.Add(new KeyValuePair<string, string>("token", updateCardData.Token));
            keyValues.Add(new KeyValuePair<string, string>("url", updateCardData.Url));
            keyValues.Add(new KeyValuePair<string, string>("language", updateCardData.Language));
            keyValues.Add(new KeyValuePair<string, string>("HPP_CUSTOMER_EMAIL", updateCardData.Email));
            keyValues.Add(new KeyValuePair<string, string>("HPP_CUSTOMER_PHONENUMBER_MOBILE", phoneNumber));
            keyValues.Add(new KeyValuePair<string, string>("HPP_BILLING_STREET1", updateCardData.Address.Street1));
            keyValues.Add(new KeyValuePair<string, string>("HPP_BILLING_STREET2", updateCardData.Address.Street2));
            keyValues.Add(new KeyValuePair<string, string>("HPP_BILLING_STREET3", updateCardData.Address.Street2));
            keyValues.Add(new KeyValuePair<string, string>("HPP_BILLING_CITY", updateCardData.Address.City));
            keyValues.Add(new KeyValuePair<string, string>("HPP_BILLING_POSTALCODE", updateCardData.Address.ZipOrPostalCode));
            keyValues.Add(new KeyValuePair<string, string>("HPP_BILLING_COUNTRY", updateCardData.CountryISOCodeNumeric));
            HttpContent content = new FormUrlEncodedContent(keyValues);

            var response = await httpClient.PostAsync(httpClient.BaseAddress, content);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entity;

            var value = await response.Content.ReadAsStringAsync();
            return value;
        }

    }
}
