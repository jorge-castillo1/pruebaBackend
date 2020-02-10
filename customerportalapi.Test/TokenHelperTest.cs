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
        public void Generar_NuevoToken_NoProduceError()
        {
            //Arrange
            string username = "P37383940Q";
            string email = "testuser@gmail.com";
            string role = "Application/bluespace_test";
            DateTime expirationDate = DateTime.UtcNow.AddMinutes(Convert.ToInt32(20));

            //Act
            string newToken = Security.JwtTokenHelper.GenerateToken(_configurations, username, role, email, expirationDate);

            //Assert
            Assert.IsFalse(string.IsNullOrEmpty(newToken));
        }

        [TestMethod]
        public void Decodificar_TokenValido_NoProduceError()
        {
            //Arrange
            string username = "P37383940Q";
            string email = "testuser@gmail.com";
            string role = "Application/bluespace_test";
            DateTime expirationdatevalue = DateTime.UtcNow.AddDays(3);
            string token = Security.JwtTokenHelper.GenerateToken(_configurations, username, role, email, expirationdatevalue);

            //Act
            ClaimsPrincipal claims = Security.JwtTokenHelper.GetPrincipal(token, _configurations);

            //Asert
            Claim name = claims.FindFirst(x => x.Type == ClaimTypes.NameIdentifier.ToString());
            Assert.AreEqual("P37383940Q", name.Value);

            Claim mail = claims.FindFirst(x => x.Type == ClaimTypes.Email.ToString());
            Assert.AreEqual("testuser@gmail.com", mail.Value);

            Claim roleclaim = claims.FindFirst(x => x.Type == ClaimTypes.Role.ToString());
            Assert.AreEqual("Application/bluespace_test", roleclaim.Value);

            Claim expirationdate = claims.FindFirst(x => x.Type == ClaimTypes.Expiration.ToString());
            Assert.AreEqual(expirationdatevalue.ToString(), expirationdate.Value);
        }

        [TestMethod]
        public void Decodificar_TokenCaducado_ProduceError()
        {
            //Arrange
            string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJQMzczODM5NDBRIiwicm9sZSI6IkFwcGxpY2F0aW9uL2JsdWVzcGFjZV90ZXN0IiwiZW1haWwiOiJ0ZXN0dXNlckBnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL2V4cGlyYXRpb24iOiIwNC8wMi8yMDIwIDE4OjEwOjQ0IiwibmJmIjoxNTgwNTgwNjQ0LCJleHAiOjE1ODA4Mzk4NDQsImlhdCI6MTU4MDU4MDY0NH0.Cpe1BccSmnuaxEQnzctEt-hIFdyJ_auOxLvM9Tlv_Bk";

            //Asert
            Assert.ThrowsException<SecurityTokenExpiredException>(
                new Action(() => {
                    Security.JwtTokenHelper.GetPrincipal(token, _configurations);
                }));
        }

        [TestMethod]
        public void GenerateKey()
        {
            var hmac = new System.Security.Cryptography.HMACSHA256();
            var key = Convert.ToBase64String(hmac.Key);

            //Assert
            Assert.IsFalse(string.IsNullOrEmpty(key));
        }
    }
}
