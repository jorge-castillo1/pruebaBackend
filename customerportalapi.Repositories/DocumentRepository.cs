using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;

namespace customerportalapi.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public DocumentRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<List<DocumentMetadata>> Search(DocumentMetadataSearchFilter filter)
        {
            var httpClient = _clientFactory.CreateClient("httpClient");
            var method = new HttpMethod("GET");

            var url = new Uri(_configuration["GatewayDocumentsUrl"] + _configuration["DocumentsSearchEndpoint"]);
            var request = new HttpRequestMessage(method, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(filter))
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode) return new List<DocumentMetadata>();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            return JsonConvert.DeserializeObject<List<DocumentMetadata>>(result.GetValue("result").ToString());
        }

        public async Task<string> SaveDocumentAsync(Document document)
        {
            var httpClient = _clientFactory.CreateClient("httpClientDocument");

            var url = new Uri(httpClient.BaseAddress + _configuration["DocumentsAPI"]);
            var postContent = new StringContent(JsonConvert.SerializeObject(document), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, postContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var documentId = result.GetValue("result").ToString();
            return documentId;
        }

        public async Task<string> SaveBlobAsync(Document document)
        {
            var httpClient = _clientFactory.CreateClient("httpClientDocument");

            var url = new Uri(httpClient.BaseAddress + _configuration["BlobAPI"]);
            var postContent = new StringContent(JsonConvert.SerializeObject(document), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, postContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var documentId = result.GetValue("result").ToString();
            return documentId;
        }

        public async Task<string> GetDocumentAsync(string documentid)
        {
            string entity = null;

            var httpClient = _clientFactory.CreateClient("httpClientDocument");
            httpClient.BaseAddress = new Uri(_configuration["GatewayDocumentsUrl"] + _configuration["DocumentsAPI"]);

            var response = await httpClient.GetAsync(documentid, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return "";
                return entity;
            }
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var contractFile = result.GetValue("result").ToString();
            return contractFile;
        }
    }
}
