using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using customerportalapi.Repositories.interfaces;

namespace customerportalapi.Repositories
{
    public class SitesRepository : ISitesRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public SitesRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<List<SmSite>> GetSmSitesAsync()
        {
            var httpClient = _clientFactory.CreateClient("httpClientSM");
            var uri = new Uri(_configuration["SmUrl"] + _configuration["SitesAPI"]);
            httpClient.BaseAddress = uri;

            var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<List<SmSite>>(result.GetValue("result").ToString());
        }
    }
}
