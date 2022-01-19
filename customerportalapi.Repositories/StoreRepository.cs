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
    public class StoreRepository : IStoreRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public StoreRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<List<Store>> GetStoresAsync()
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            var uri = new Uri(_configuration["GatewayUrl"] + _configuration["StoresAPI"]);
            httpClient.BaseAddress = uri;

            var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new List<Store>();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<List<Store>>(result.GetValue("result").ToString()); ;
        }

        public async Task<Store> GetStoreAsync(string storeCode)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["StoresAPI"]);

            var response = await httpClient.GetAsync(storeCode, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new Store();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<Store>(result.GetValue("result").ToString());
        }

        public async Task<Unit> GetUnitAsync(Guid id)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["UnitsAPI"]);

            var response = await httpClient.GetAsync(id.ToString(), HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new Unit();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<Unit>(result.GetValue("result").ToString());
        }

        public async Task<Unit> GetUnitBySMIdAsync(string smid)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["UnitsAPI"]);

            var response = await httpClient.GetAsync(smid, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new Unit();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<Unit>(result.GetValue("result").ToString());
        }

        public async Task<Store> UpdateSiteImage(StoreImageUrl storeImageUrl)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, new Uri(_configuration["GatewayUrl"] + _configuration["StoresAPI"]))
            {
                Content = new StringContent(JsonConvert.SerializeObject(storeImageUrl), Encoding.UTF8, "application/json")
            };
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<Store>(result.GetValue("result").ToString());
        }
    }
}
