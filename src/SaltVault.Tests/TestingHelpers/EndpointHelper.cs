using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using SaltVault.WebApp;

namespace SaltVault.Tests.TestingHelpers
{
    public class EndpointHelper
    {
        public static HttpClient CreateFakeServer(string sessionId)
        {
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient httpClient = server.CreateClient();
            //httpClient.BaseAddress = new Uri("http://localhost:5124/Api/v2/Bills");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", sessionId);
            return httpClient;
        }
    }
}