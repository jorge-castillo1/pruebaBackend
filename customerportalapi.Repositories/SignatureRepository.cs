using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

//using AutoWrapper.Wrappers;

namespace customerportalapi.Repositories
{
    public class SignatureRepository : ISignatureRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        private readonly IMongoCollectionWrapper<SignatureResultData> _signatureResult;

        public SignatureRepository(IConfiguration configuration, IHttpClientFactory clientFactory, IMongoCollectionWrapper<SignatureResultData> signatureResult)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _signatureResult = signatureResult;
        }

        public async Task<Guid> CreateSignature(MultipartFormDataContent form)
        {
            var httpClient = _clientFactory.CreateClient("httpClientSignature");
            var url = httpClient.BaseAddress + _configuration["SignatureEndpoint"];

            var response = await httpClient.PostAsync(url, form);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var deserializedContent = JsonConvert.DeserializeObject<SignatureResponse>(content);
            return deserializedContent.Result.Documents[0].Id;
        }

        public async Task<bool> CancelSignature(string id)
        {
            var httpClient = _clientFactory.CreateClient("httpClientSignature");
            var url = httpClient.BaseAddress + _configuration["SignatureEndpoint"] + id;

            var response = await httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();

            await response.Content.ReadAsStringAsync();
            return true;
        }

        public async Task<List<SignatureProcess>> SearchSignaturesAsync(SignatureSearchFilter filter)
        {
            var httpClient = _clientFactory.CreateClient("httpClientSignature");
            var url = httpClient.BaseAddress + _configuration["SignatureEndpoint"];

            var content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var request = new HttpRequestMessage(new HttpMethod("GET"), url)
            {
                Content = content
            };

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var contentresult = await response.Content.ReadAsStringAsync();

            var deserializedContent = JsonConvert.DeserializeObject<SignatureSearchResponse>(contentresult);
            return deserializedContent.Result;
        }

        public async Task<List<SignatureResultData>> GetSignatureInfoAsync(string contractNumber, string fromDate, string documentCountry, string status = null)
        {
            var httpClient = _clientFactory.CreateClient("httpClientSignature");
            httpClient.BaseAddress = new Uri($"{httpClient.BaseAddress}{_configuration["SignatureEndpoint"]}");
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            var url = $"{contractNumber}/{fromDate}";
            if (!string.IsNullOrEmpty(documentCountry))
            {
                url += $"/{documentCountry}";
                if (!string.IsNullOrEmpty(status))
                {
                    url += $"/{status}";
                }
            }

            var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var contentresult = await response.Content.ReadAsStringAsync();

            var deserializedContent = JsonConvert.DeserializeObject<SignatureResultDataListResponse>(contentresult);
            return deserializedContent.Result;
        }

        public async Task<string> UploadDocumentAsync(DocumentMetadata metadata, string documentCountry, string since, string status)
        {
            var httpClient = _clientFactory.CreateClient("httpClientSignature");
            httpClient.Timeout = TimeSpan.FromMinutes(10);

            //[HttpPatch("UploadDocument/{documentCountry}/{since}/{status}")]
            var url = $"{httpClient.BaseAddress}{_configuration["SignatureEndpoint"]}UploadDocument/{documentCountry}";
            if (!string.IsNullOrEmpty(since))
            {
                url += $"/{since}";

                if (!string.IsNullOrEmpty(status))
                    url += $"/{status}";
            }

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), new Uri(url))
            {
                Content = new StringContent(JsonConvert.SerializeObject(metadata), Encoding.UTF8, "application/json")
            };
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            JObject result = JObject.Parse(content);
            var documentId = result.GetValue("result")?.ToString();
            return documentId;
        }

        public Task<bool> Create(SignatureResultData signatureResult)
        {
            _signatureResult.InsertOne(signatureResult);

            return Task.FromResult(true); ;
        }
    }
}