using System;
using System.Collections.Generic;
using System.Text;
using customerportalapi.Repositories.interfaces;
using System.Threading.Tasks;
using System.Net.Http;
using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
//using AutoWrapper.Wrappers;


namespace customerportalapi.Repositories
{
    public class SignatureRepository : ISignatureRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public SignatureRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
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
    }
}
