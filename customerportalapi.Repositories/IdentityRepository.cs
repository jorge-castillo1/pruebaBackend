using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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

        public async Task<Token> Validate(string token)
        {
            var httpClient = _clientFactory.CreateClient("identityClient");
            try
            {
                var body = new Dictionary<string, string>();
                body.Add("token", token);
                var form = new FormUrlEncodedContent(body);

                var url = httpClient.BaseAddress + _configuration["Identity:Endpoints:Validate"];
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                        String.Format("{0}:{1}",_configuration["Identity:Credentials:User"], _configuration["Identity:Credentials:Password"])))
                );
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
