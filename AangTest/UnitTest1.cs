using Aang.Model.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;

namespace AangTest
{
    public class UnitTest1 : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private AppTestContext _appContext;

        public UnitTest1(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        private void SetContext()
        {
            var provider = _factory.Provider.CreateScope().ServiceProvider;
            _appContext = provider.GetRequiredService<AppTestContext>();
        }

        private void Seeding()
        {
            _appContext.Database.EnsureDeleted();
            _appContext.Database.EnsureCreated();
            _appContext.SeedData();
        }

        private string GetSecretKey()
        {
            var configuration = _factory.Provider.GetRequiredService<IConfiguration>();
            return configuration["SecretKey"] ?? "Bananerama";
        }

        private string GetToken(string secretKey)
        {
            var login = new LoginRequestDTO()
            {
                Username = "testi",
                Password = "testi123!DE",
            };

            var tokenHelpr = new TokenHelper(_appContext);
            return tokenHelpr.Login(login, secretKey);
        }

        [Fact]
        public async void TestUserInDB()
        {
            // Act
            SetContext();
            Seeding();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(GetSecretKey()));

            var response = await _client.GetAsync("/weatherforecast");
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}