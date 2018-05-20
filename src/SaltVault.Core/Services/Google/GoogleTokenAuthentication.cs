using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace SaltVault.Core.Services.Google
{
    public interface IGoogleTokenAuthentication
    {
        GoogleTokenInformation VerifyToken(string token);
    }

    public class GoogleTokenAuthentication : IGoogleTokenAuthentication
    {
        private readonly HttpClient _googleTokenClient;
        private readonly string _appClientId;

        public GoogleTokenAuthentication(HttpClient httpClient)
        {
            _googleTokenClient = httpClient;
            _googleTokenClient.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v3/");
            _appClientId = File.ReadAllText("./Data/Config/ClientIds.config");
        }

        public GoogleTokenInformation VerifyToken(string token)
        {
            var response = _googleTokenClient.GetAsync("tokeninfo?id_token=" + token).Result;
            string responseContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode != HttpStatusCode.OK)
                return new GoogleTokenInformation { Valid = false };
            
            GoogleTokenInformation tokenInfo = JsonConvert.DeserializeObject<GoogleTokenInformation>(responseContent);
            if (AuthenticateClientId(tokenInfo.aud) == false)
                return new GoogleTokenInformation { Valid = false };

            tokenInfo.Valid = true;
            return tokenInfo;
        }

        private bool AuthenticateClientId(string clientId)
        {
            try
            {
                string sanitisedToken = clientId.Replace("ClientID ", "");
                return _appClientId.Contains(sanitisedToken);
            }
            catch (System.Exception exception)
            {
                return false;
            }
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
