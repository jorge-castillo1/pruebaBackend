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
    public class OpportunityCRMRepository : IOpportunityCRMRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public OpportunityCRMRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<OpportunityCRM> GetOpportunity(string opportunityId)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            var uri = new Uri(_configuration["GatewayUrl"] + _configuration["OpportunityCRM"]);
            httpClient.BaseAddress = uri;

            var response = await httpClient.GetAsync(opportunityId, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new OpportunityCRM();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<OpportunityCRM>(result.GetValue("result").ToString());
        }
    }
}
