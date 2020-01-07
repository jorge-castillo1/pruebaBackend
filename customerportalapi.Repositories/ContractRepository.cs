﻿using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

namespace customerportalapi.Repositories
{
    public class ContractRepository : IContractRepository
    {
        readonly IConfiguration _configuration;
        readonly IHttpClientFactory _clientFactory;

        public ContractRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<List<Contract>> GetContractsAsync(string dni)
        {
            var entitylist = new List<Contract>();
            
            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ContractsAPI"]);

            var response = await httpClient.GetAsync(dni, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entitylist;
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var contractList = JsonConvert.DeserializeObject<List<Contract>>(result.GetValue("result").ToString());

            return contractList;
        }
    }
}
