using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
    }
}
