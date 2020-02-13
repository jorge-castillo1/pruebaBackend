using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Logging;

namespace customerportalapi.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ProfileRepository> _logger;

        public ProfileRepository(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<ProfileRepository> logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<Profile> GetProfileAsync(string dni, string accountType)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ProfileAPI"]);
            _logger.LogWarning("Base Address: " + httpClient.BaseAddress.AbsolutePath);

            try
            {
                var response = await httpClient.GetAsync(dni + "/" + accountType, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(content);

                return JsonConvert.DeserializeObject<Profile>(result.GetValue("result").ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new Profile();
            }
        }

        public async Task<Profile> UpdateProfileAsync(Profile profile)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, new Uri(_configuration["GatewayUrl"] + _configuration["ProfileAPI"]))
            {
                Content = new StringContent(JsonConvert.SerializeObject(profile), Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<Profile>(result.GetValue("result").ToString());
        }

        public async Task<AccountProfile> GetAccountAsync(string dni, string accountType)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["AccountsAPI"]);

            var response = await httpClient.GetAsync(dni + "/" + accountType, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<AccountProfile>(result.GetValue("result").ToString());
        }

        public async Task<AccountProfile> UpdateAccountAsync(AccountProfile account)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, new Uri(_configuration["GatewayUrl"] + _configuration["AccountsAPI"]))
            {
                Content = new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<AccountProfile>(result.GetValue("result").ToString());
        }

        public async Task<Profile> ConfirmedWebPortalAccessAsync(string dni, string accountType)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, new Uri(_configuration["GatewayUrl"] + _configuration["ProfileAPI"] + $"{dni}/{accountType}/invited"));

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<Profile>(result.GetValue("result").ToString());
        }

        public async Task<Profile> RevokedWebPortalAccessAsync(string dni, string accountType)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, new Uri(_configuration["GatewayUrl"] + _configuration["ProfileAPI"] + $"{dni}/{accountType}/uninvited"));
            
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<Profile>(result.GetValue("result").ToString());
        }

        public async Task<ProfilePermissions> GetProfilePermissionsAsync(string dni, string accountType)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ProfileAPI"]);
            _logger.LogWarning("Base Address: " + httpClient.BaseAddress.AbsolutePath);

            try
            {
                var response = await httpClient.GetAsync(dni + "/" + accountType, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(content);
                return JsonConvert.DeserializeObject<ProfilePermissions>(result.GetValue("result").ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ProfilePermissions();
            }
        }
    }
}
