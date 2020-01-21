﻿using customerportalapi.Repositories.interfaces;
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
        readonly IConfiguration _configuration;
        readonly IHttpClientFactory _clientFactory;

        private readonly ILogger<ProfileRepository> _logger;

        public ProfileRepository(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<ProfileRepository> logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<Profile> GetProfileAsync(string dni)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ProfileAPI"]);
            _logger.LogWarning("Base Address: " + httpClient.BaseAddress.AbsolutePath);
            try
            {
               var response = await httpClient.GetAsync(dni, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                if (!response.IsSuccessStatusCode) return new Profile();
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
            if (!response.IsSuccessStatusCode) return new Profile();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<Profile>(result.GetValue("result").ToString());
        }
    }
}
