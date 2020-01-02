using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

namespace customerportalapi.Repositories
{
    public class ContactRepository : IContactRepository
    {
        readonly IConfiguration _configuration;
        readonly IHttpClientFactory _clientFactory;

        public ContactRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<Contact> GetContactAsync(string dni)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ContactsAPI"]);

            var response = await httpClient.GetAsync(dni, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new Contact();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<Contact>(result.GetValue("result").ToString());
        }

        public async Task<Contact> UpdateContactAsync(Contact contact)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, new Uri(_configuration["GatewayUrl"] + _configuration["ContactsAPI"]))
            {
                Content = new StringContent(JsonConvert.SerializeObject(contact), Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new Contact();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<Contact>(result.GetValue("result").ToString());
        }
    }
}
