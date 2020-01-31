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
                body.Add("grant_type", "passowrd");
                body.Add("scope", "openid");
                var form = new FormUrlEncodedContent(body);
                var url = httpClient.BaseAddress;
                var response = await httpClient.PostAsync(url, form);
                response.EnsureSuccessStatusCode();
                if (!response.IsSuccessStatusCode) 
                    return new Token();

                var content = await response.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(content);

                return JsonConvert.DeserializeObject<Token>(result.GetValue("result").ToString());
            }
            catch (Exception ex)
            {
                return new Token();
            }
        }
    }
}
