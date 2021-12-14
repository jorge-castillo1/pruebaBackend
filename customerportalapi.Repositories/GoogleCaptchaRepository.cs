using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace customerportalapi.Repositories
{
    public class GoogleCaptchaRepository : IGoogleCaptchaRepository
    {
        readonly IConfiguration _configuration;
        readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<GoogleCaptchaRepository> _logger;


        public GoogleCaptchaRepository(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<GoogleCaptchaRepository> logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<bool> IsTokenValid(string responseToken)
        {
            bool result = false;

            using (var form = new MultipartFormDataContent())
            {
                var httpClient = _clientFactory.CreateClient("httpClientCaptcha");

                var url = new Uri(_configuration["GoogleCaptchaService:Url"]);

                form.Add(new StringContent(_configuration["GoogleCaptchaService:Secret"]), "secret");

                form.Add(new StringContent(responseToken), "response");

                //grabar en el log la llamada
                _logger.LogInformation($"GoogleCaptchaRepository.IsTokenValid(). Method POST. Url:{url.AbsoluteUri}, secretToken: {_configuration["GoogleCaptchaService:Secret"]}, responseToken: {responseToken}");

                var response = await httpClient.PostAsync(url, form);
                if (response.IsSuccessStatusCode)
                {
                    // 200 OK                   
                    var content = await response.Content.ReadAsStringAsync();
                    //grabar en log respuesta
                    _logger.LogInformation($"GoogleCaptchaRepository.IsTokenValid(). Response: {content}");

                    var objectResponse = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(content);
                    result = objectResponse.Success;
                }
                else
                {
                    // Grabar el Error en el log
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"GoogleCaptchaRepository.IsTokenValid(). Response: {content}");
                }
            }
            return result;
        }
    }
}
