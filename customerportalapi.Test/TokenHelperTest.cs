using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace customerportalapi.Test
{
    [TestClass]
    public class TokenHelperTest
    {
        IConfigurationRoot _configurations;

        [TestInitialize]
        public void ConfigurarValores()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            _configurations = builder.Build();
        }

        [TestMethod]
        public void Decodificar_Token_NoProduceError()
        {
            //Arrange
            string token = "eyJ4NXQiOiJNV1JtTkRJeE9URTJaREJrWW1SaVptRmhOekkwWlRobU1tRXhZbUUyWW1JMk9UYzFNV1ppWXciLCJraWQiOiJNV1JtTkRJeE9URTJaREJrWW1SaVptRmhOekkwWlRobU1tRXhZbUUyWW1JMk9UYzFNV1ppWXciLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiJYODAyODkxNkZAY2FyYm9uLnN1cGVyIiwiYXVkIjoiajRlbjZaT2k2MHVIMjFWbEpEOHZ6cGZwMlJBYSIsIm5iZiI6MTU4MDkwMjcwNywiYXpwIjoiajRlbjZaT2k2MHVIMjFWbEpEOHZ6cGZwMlJBYSIsInNjb3BlIjoib3BlbmlkIiwiaXNzIjoiaHR0cHM6XC9cL2lkZW50aXR5LXByZS5ibHVlc3BhY2UuZXU6OTQ0M1wvb2F1dGgyXC90b2tlbiIsImdyb3VwcyI6WyJjdXN0b21lcnBvcnRhbF9zdGFuZGFyZCIsIkludGVybmFsXC9ldmVyeW9uZSJdLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJYODAyODkxNkYiLCJleHAiOjE1ODA5MDYzMDcsImlhdCI6MTU4MDkwMjcwNywianRpIjoiNDkwMzE5YjMtNTFmYi00M2Y5LWFjYmYtYjY2YmY5MWFlNWMxIiwiZW1haWwiOiJ0ZW8ucXVpcm96QHF1YW50aW9uLmNvbSJ9.Xs4bF4voEBU8-eE3LCTjvqKSS5wk8EKkdmR_Qdz6-q_q6DjtJ-jjReFtQEPJ6kkSxm9jMIXwCGsnz7DSk-mD9EJRsDWMjP-LnKIaGAnaTnDXLxd7j8RXBpO37sWcDVyjFRkpyX59kff7qa_4rBhc48UMk3oPxrqaA346cmYMR3dZOvENuwwF23eBpIRAyrkl85FQsdPgSkc61oMPr_smI34VZGXsJVFX-r4byGXy_40fvdFV4rKa7EIZQh2NvLEYQpPE5BS_dSGDCDamupRu_PGKk5FmMCNv3CwDvY6-OYcZU80Mc63uuBLmFVCATfTj2dus_pfRTmb7KSpR4UErFw";

            //Act
            ClaimsPrincipal claims = Security.JwtTokenHelper.GetPrincipal(token, _configurations);

            //Asert
            Claim name = claims.FindFirst(x => x.Type == ClaimTypes.Name.ToString());
            Assert.AreEqual("X8028916F", name.Value);

            Claim mail = claims.FindFirst(x => x.Type == ClaimTypes.Email.ToString());
            Assert.AreEqual("teo.quiroz@quantion.com", mail.Value);

            Claim roleclaim = claims.FindFirst(x => x.Type == ClaimTypes.Role.ToString());
            Assert.AreEqual("customerportal_standard", roleclaim.Value);

            Claim expirationdate = claims.FindFirst(x => x.Type == ClaimTypes.Expiration.ToString());
            Assert.IsNotNull(expirationdate.Value);
        }
    }
}
