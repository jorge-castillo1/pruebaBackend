﻿using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public ContractRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<List<Contract>> GetContractsAsync(string dni, string accountType)
        {
            var entitylist = new List<Contract>();

            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ContractsAPI"]);

            var response = await httpClient.GetAsync(dni + "/" + accountType, HttpCompletionOption.ResponseHeadersRead);
            //response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entitylist;
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var contractList = JsonConvert.DeserializeObject<List<Contract>>(result.GetValue("result").ToString());

            return contractList;
        }

        public async Task<Contract> GetContractAsync(string smContractCode)
        {
            var entity = new Contract();

            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ContractsAPI"]);

            var response = await httpClient.GetAsync("code/" + smContractCode, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entity;
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var contract = JsonConvert.DeserializeObject<Contract>(result.GetValue("result").ToString());

            return contract;
        }

        public async Task<Contract> UpdateContractAsync(Contract cont)
        {
            var entity = new Contract();

            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, new Uri(_configuration["GatewayUrl"] + _configuration["ContractsAPI"]))
            {
                Content = new StringContent(JsonConvert.SerializeObject(cont), Encoding.UTF8, "application/json")
            };
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);

            return JsonConvert.DeserializeObject<Contract>(result.GetValue("result").ToString());
        }

        public async Task<List<FullContract>> GetFullContractsWithoutUrlAsync(int? limit)
        {
            var entitylist = new List<FullContract>();

            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ContractsAPI"]);

            string endPoint = "fullcontracts";
            if (limit.HasValue && limit >= 1)
                endPoint = $"fullcontracts?limit={limit.Value}";

            var response = await httpClient.GetAsync(endPoint, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entitylist;
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var contractList = JsonConvert.DeserializeObject<List<FullContract>>(result.GetValue("result").ToString());

            return contractList;
        }

        public async Task<List<FullContract>> GetFullContractsBySMCodeAsync(string code)
        {
            var entitylist = new List<FullContract>();

            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ContractsAPI"]);

            var endPoint = $"fullcontracts?smcode={code}";

            var response = await httpClient.GetAsync(endPoint, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entitylist;
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var contractList = JsonConvert.DeserializeObject<List<FullContract>>(result.GetValue("result").ToString());

            return contractList;
        }

        public async Task<FullContract> GetFullContractsByCRMCodeAsync(string crmCode)
        {
            var entitylist = new FullContract();

            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ContractsAPI"]);

            var endPoint = $"fullcontractscrmcode?crmcode={crmCode}";

            var response = await httpClient.GetAsync(endPoint, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entitylist;
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var contractList = JsonConvert.DeserializeObject<FullContract>(result.GetValue("result").ToString());

            return contractList;
        }

        public async Task<List<FullContract>> GetFullContractsWithoutSignaturitId(string fromCreatedOn, string toCreatedOn = null)
        {
            var entitylist = new List<FullContract>();

            var httpClient = _clientFactory.CreateClient("httpClientCRM");
            httpClient.BaseAddress = new Uri(_configuration["GatewayUrl"] + _configuration["ContractsAPI"]);
            //httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

            var url = $"{_configuration["GatewayUrl"]}{_configuration["ContractsAPI"]}";
            httpClient.BaseAddress = new Uri(url);
            if (!string.IsNullOrEmpty(toCreatedOn))
            {
                url += $"&toCreatedOn={toCreatedOn}";
            }

            var response = await httpClient.GetAsync($"withoutsignaturitid?fromCreatedOn={fromCreatedOn}", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return entitylist;
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var contractList = JsonConvert.DeserializeObject<List<FullContract>>(result.GetValue("result").ToString());

            return contractList;
        }
    }
}