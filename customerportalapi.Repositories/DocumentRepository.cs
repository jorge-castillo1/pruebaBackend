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

        public async Task<string> SaveDocumentBlobStorageUnitImageContainerAsync(Document document)
        {
            return await SaveDocumentBlobStorageContainerAsync(document, _configuration["BlobStorageUnitImageContainer"]);
        }

        public async Task<string> SaveDocumentBlobStorageStoreFacadeImageContainerAsync(Document document)
        {
            return await SaveDocumentBlobStorageContainerAsync(document, _configuration["BlobStorageStoreFacadeImageContainer"]);
        }

        private async Task<string> SaveDocumentBlobStorageContainerAsync(Document document, string containerName)
        {
            var httpClient = _clientFactory.CreateClient("httpClientDocument");

            var url = new Uri($"{httpClient.BaseAddress}{_configuration["BlobAPI"]}{containerName}");
            var postContent = new StringContent(JsonConvert.SerializeObject(document), Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_configuration["CustomerPortal_ApiKey"]);
            var response = await httpClient.PostAsync(url, postContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var documentId = result.GetValue("result").ToString();
            return documentId;
        }

        public async Task<string> DeleteDocumentBlobStorageStoreFacadeImageContainerAsync(string path)
        {
            return await DeleteDocumentBlobStorageContainerAsync(path, _configuration["BlobStorageStoreFacadeImageContainer"]);
        }

        private async Task<string> DeleteDocumentBlobStorageContainerAsync(string path, string containerName)
        {
            var httpClient = _clientFactory.CreateClient("httpClientDocument");
            var url = new Uri($"{httpClient.BaseAddress}{_configuration["BlobAPI"]}{containerName}?path={path}&container={containerName}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_configuration["CustomerPortal_ApiKey"]);
            var response = await httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            var documentId = result.GetValue("result").ToString();
            return documentId;
        }

        public async Task<BlobResult> GetDocumentBlobStorageUnitImageAsync(string path)
        {
            return await GetDocumentBlobStorageAsync(path, _configuration["BlobStorageUnitImageContainer"]);
        }

        public async Task<BlobResult> GetDocumentBlobStorageStoreFacadeImageAsync(string path)
        {
            return await GetDocumentBlobStorageAsync(path, _configuration["BlobStorageStoreFacadeImageContainer"]);
        }

        private async Task<BlobResult> GetDocumentBlobStorageAsync(string path, string containerName)
        {
            var httpClient = _clientFactory.CreateClient("httpClientDocument");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_configuration["CustomerPortal_ApiKey"]);

            httpClient.BaseAddress = new Uri($"{httpClient.BaseAddress}{_configuration["BlobAPI"]}{containerName}/info");

            var response = await httpClient.GetAsync("?path=" + path, HttpCompletionOption.ResponseHeadersRead);

            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            return JsonConvert.DeserializeObject<BlobResult>(result.GetValue("result").ToString());
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

        public async Task<Document> GetFullDocumentAsync(string documentid)
        {
            var httpClient = _clientFactory.CreateClient("httpClientDocument");
            httpClient.BaseAddress = new Uri(_configuration["GatewayDocumentsUrl"] + _configuration["DocumentsAPI"]);

            var response = await httpClient.GetAsync(documentid, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(content);
            return JsonConvert.DeserializeObject<Document>(result.GetValue("result").ToString());
        }
    }
}
