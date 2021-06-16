using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

        public async Task<TokenStatus> Validate(string token)
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
                        String.Format("{0}:{1}", _configuration["Identity:Credentials:User"], _configuration["Identity:Credentials:Password"])))
                );
                var response = await httpClient.PostAsync(url, form);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TokenStatus>(content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JwtSecurityToken ValidateAzuereAD(string token)
        {
            var jwt = (SecurityToken)new JwtSecurityToken();
            try
            {
#if DEBUG
                IdentityModelEventSource.ShowPII = true;
#endif
                var myAudience = _configuration["AzureAd:ClientId"];
                var myIssuer = String.Format(CultureInfo.InvariantCulture, "{0}/{1}/", _configuration["AzureAd:Instance"], _configuration["AzureAd:TenantId"]);
                var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(_configuration["AzureAd:DiscoveryEndpoint"], new OpenIdConnectConfigurationRetriever());
                var config = configManager.GetConfigurationAsync().Result;

                var validationParameters = new TokenValidationParameters
                {
                    ValidAudience = myAudience,
                    ValidIssuer = myIssuer,
                    IssuerSigningKeys = config.SigningKeys,
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var result = tokenHandler.ValidateToken(token, validationParameters, out jwt);
                return jwt as JwtSecurityToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserIdentity> AddUser(UserIdentity userIdentity)
        {
            var httpClient = _clientFactory.CreateClient("identityClient");
            var method = new HttpMethod("POST");
            try
            {
                var url = new Uri(httpClient.BaseAddress + _configuration["Identity:Endpoints:Provisioning"]);
                var postContent = new StringContent(JsonConvert.SerializeObject(userIdentity), Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                        String.Format("{0}:{1}", _configuration["Identity:Credentials:User"], _configuration["Identity:Credentials:Password"])))
                );
                var response = await httpClient.PostAsync(url, postContent);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserIdentity>(content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserIdentity> UpdateUser(UserIdentity userIdentity)
        {
            var httpClient = _clientFactory.CreateClient("identityClient");
            try
            {
                var url = new Uri(httpClient.BaseAddress + _configuration["Identity:Endpoints:Provisioning"] + $"/{userIdentity.ID}");
                var postContent = new StringContent(JsonConvert.SerializeObject(userIdentity), Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                        String.Format("{0}:{1}", _configuration["Identity:Credentials:User"], _configuration["Identity:Credentials:Password"])))
                );
                var response = await httpClient.PutAsync(url, postContent);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserIdentity>(content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserIdentity> GetUser(string userId)
        {
            var httpClient = _clientFactory.CreateClient("identityClient");
            try
            {
                var url = new Uri(httpClient.BaseAddress + _configuration["Identity:Endpoints:Provisioning"] + $"/{userId}");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                        String.Format("{0}:{1}", _configuration["Identity:Credentials:User"], _configuration["Identity:Credentials:Password"])))
                );
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserIdentity>(content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteUser(string userId)
        {
            var httpClient = _clientFactory.CreateClient("identityClient");
            try
            {
                var url = new Uri(httpClient.BaseAddress + _configuration["Identity:Endpoints:Provisioning"] + $"/{userId}");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                        String.Format("{0}:{1}", _configuration["Identity:Credentials:User"], _configuration["Identity:Credentials:Password"])))
                );
                var response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GroupResults> FindGroup(string groupName)
        {
            var httpClient = _clientFactory.CreateClient("identityClient");
            try
            {
                var url = new Uri(httpClient.BaseAddress + _configuration["Identity:Endpoints:Groups"] + $"?&filter=displayName eq PRIMARY/{groupName}");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                        String.Format("{0}:{1}", _configuration["Identity:Credentials:User"], _configuration["Identity:Credentials:Password"])))
                );
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GroupResults>(content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserIdentity> AddUserToGroup(UserIdentity userIdentity, Group group)
        {
            var httpClient = _clientFactory.CreateClient("identityClient");
            var method = new HttpMethod("PATCH");
            try
            {
                var url = new Uri(httpClient.BaseAddress + _configuration["Identity:Endpoints:Groups"] + $"/{group.ID}");

                UserGroupOperations operations = new UserGroupOperations();
                UserGroupOperation addoperation = new UserGroupOperation();
                addoperation.Operation = "add";
                addoperation.Value = new UserGroupOperationValue()
                {
                    Members = new List<UserGroupMember>()
                    {
                        new UserGroupMember()
                        {
                            Display = userIdentity.UserName,
                            Value = userIdentity.ID
                        }
                    }
                };
                operations.Operations.Add(addoperation);
                var request = new HttpRequestMessage(method, url)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(operations))
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                        String.Format("{0}:{1}", _configuration["Identity:Credentials:User"], _configuration["Identity:Credentials:Password"])))
                );
                //var response = await httpClient.PutAsync(url, postContent);
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserIdentity>(content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Token> RefreshToken(string refreshToken)
        {
            var httpClient = _clientFactory.CreateClient("identityClient");
            try
            {
                var body = new Dictionary<string, string>();
                body.Add("refresh_token", refreshToken);
                body.Add("grant_type", "refresh_token");
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

        public async Task<bool> Logout(string token)
        {
            var httpClient = _clientFactory.CreateClient("identityClient");
            try
            {
                var body = new Dictionary<string, string>();
                body.Add("token", token);
                body.Add("token_type_hint", "access_token");
                var form = new FormUrlEncodedContent(body);
                var url = httpClient.BaseAddress + _configuration["Identity:Endpoints:Revoke"];
                var response = await httpClient.PostAsync(url, form);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveUserFromGroup(UserIdentity userIdentity, Group group)
        {
            var httpClient = _clientFactory.CreateClient("identityClient");
            var method = new HttpMethod("PATCH");
            try
            {
                var url = new Uri(httpClient.BaseAddress + _configuration["Identity:Endpoints:Groups"] + $"/{group.ID}");

                UserGroupRemoveOperations operations = new UserGroupRemoveOperations();
                UserGroupRemoveOperation removeoperation = new UserGroupRemoveOperation();
                removeoperation.Operation = "remove";
                removeoperation.Path = "members[value eq " + userIdentity.ID + "]";
                operations.Operations.Add(removeoperation);
                var request = new HttpRequestMessage(method, url)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(operations))
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                        String.Format("{0}:{1}", _configuration["Identity:Credentials:User"], _configuration["Identity:Credentials:Password"])))
                );
                //var response = await httpClient.PutAsync(url, postContent);
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
