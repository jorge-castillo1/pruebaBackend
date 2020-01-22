﻿using customerportalapi.Repositories.interfaces;
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

        public async Task<Store> GetStoreAsync(string storeId)
        {
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["StoresAPI"]);

            var response = await httpClient.GetAsync(storeId, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new Store();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<Store>(result.GetValue("result").ToString());
        }
    }
}
