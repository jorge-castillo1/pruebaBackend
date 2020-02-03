using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace customerportalapi.Repositories
{
    public class IdentityRepository : IIdentityRepository
    {
        readonly IConfiguration _configuration;
        readonly IHttpClientFactory _clientFactory;


        public IdentityRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<Token> Authorize(Login credentials)
        {
            var httpClient = _clientFactory.CreateClient("identityClient");
            try
            {

                var body = new Dictionary<string, string>();
                body.Add("username", credentials.Username);
                body.Add("password", credentials.Password);
                body.Add("grant_type", "password");
                body.Add("scope", "openid");
                var form = new FormUrlEncodedContent(body);
                var url = httpClient.BaseAddress + _configuration["Identity:Endpoints:Authorize"];
                var response = await httpClient.PostAsync(url, form);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
               return JsonConvert.DeserializeObject<Token>(content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
