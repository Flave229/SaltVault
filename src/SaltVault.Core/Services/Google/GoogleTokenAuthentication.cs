using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace SaltVault.Core.Google
{
    public class GoogleTokenAuthentication
    {
        private readonly HttpClient _googleTokenClient;
        private readonly string _googleTokenInfoEndpoint;

        public GoogleTokenAuthentication(HttpClient httpClient)
        {
            _googleTokenClient = httpClient;
            _googleTokenClient.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v3/");
            _googleTokenInfoEndpoint = "tokeninfo?id_token=";
        }

        public GoogleTokenInformation VerifyToken(string token)
        {
            var response = _googleTokenClient.GetAsync("tokeninfo?id_token=" + token).Result;
            string responseContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode != HttpStatusCode.OK)
                return new GoogleTokenInformation { Valid = false };

            GoogleTokenInformation tokenInfo = JsonConvert.DeserializeObject<GoogleTokenInformation>(responseContent);
            tokenInfo.Valid = true;
            return tokenInfo;
        }
    }

    public class GoogleTokenInformation
    {
        public bool Valid { get; set; }
        public string sub { get; set; }
        public string aud { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string picture { get; set; }
    }
}
