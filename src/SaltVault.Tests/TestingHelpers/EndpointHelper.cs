using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using SaltVault.WebApp;

namespace SaltVault.Tests.TestingHelpers
{
    public class EndpointHelper
    {
        private static HttpClient _fakeSever;

        public static HttpClient CreateFakeServer()
        {
            if (_fakeSever != null)
                return _fakeSever;

            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient httpClient = server.CreateClient();
            _fakeSever = httpClient;
            return _fakeSever;
        }

        public static void SetAuthenticationToken(string sessionId)
        {
            _fakeSever.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", sessionId);
        }
    }
}