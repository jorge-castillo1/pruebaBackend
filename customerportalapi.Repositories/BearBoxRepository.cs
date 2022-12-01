using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories
{
    public class BearBoxRepository : IBearBoxRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<BearBoxRepository> _logger;

        public BearBoxRepository(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<BearBoxRepository> logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }
        public async Task<object> GetUser(string smCustomerId)
        {
            var httpClient = _clientFactory.CreateClient("httpClientBearBox");
            httpClient.BaseAddress = new Uri($"{_configuration["BearBox:ServiceUrl"]}{_configuration["BearBox:User"]}");
            _logger.LogWarning("Base Address: " + httpClient.BaseAddress.AbsolutePath);
            try
            {
                var filter = "?filter={\"where\": {\"rentalCustomerID\": {\"like\": \"XXXX\"}},\"limit\": 100}";
                var requestUri = filter.Replace("XXXX", smCustomerId);

                var response = await httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(content);

                return Newtonsoft.Json.JsonConvert.DeserializeObject<Profile>(result.GetValue("result").ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Profile();
            }
        }

        public async Task<object> GetPIN(string userId)
        {
            var httpClient = _clientFactory.CreateClient("httpClientBearBox");
            httpClient.BaseAddress = new Uri($"{_configuration["BearBox:ServiceUrl"]}{_configuration["BearBox:User"]}");
            _logger.LogWarning("Base Address: " + httpClient.BaseAddress.AbsolutePath);
            try
            {
                var filter = "?filter={\"where\": {\"userID\": XXXX}}";
                var requestUri = filter.Replace("XXXX", userId);

                var response = await httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(content);

                return Newtonsoft.Json.JsonConvert.DeserializeObject<Profile>(result.GetValue("result").ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Profile();
            }
        }

        public async Task<object> UpdatePINAsync(object user)
        {
            var httpClient = _clientFactory.CreateClient("httpClientBearBox");
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, new Uri($"{_configuration["BearBox:ServiceUrl"]}{_configuration["BearBox:Pin"]}/id"))
            {
                Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Profile>(result.GetValue("result").ToString());
        }
    }
}
