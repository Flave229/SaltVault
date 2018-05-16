namespace SaltVault.Core.Google
{
    public class GoogleTokenAuthentication
    {
        private readonly string _googleTokenInfoEndpoint;

        public GoogleTokenAuthentication()
        {
            _googleTokenInfoEndpoint = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=";
        }
    }
}
