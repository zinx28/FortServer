namespace FortLibrary.EpicResponses.Oauth
{
    public class OauthToken
    {
        public string access_token { get; set; } = string.Empty;
        public long expires_in { get; set; }
        public string expires_at { get; set; } = string.Empty;
        public string token_type { get; set; } = string.Empty;
        public string account_id { get; set; } = string.Empty;
        public string client_id { get; set; } = string.Empty;
        public bool internal_client { get; set; }
        public string client_service { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
        public int refresh_expires { get; set; }
        public string refresh_expires_at { get; set; } = string.Empty;
        public string displayName { get; set; } = string.Empty;
        public string app { get; set; } = string.Empty;
        public string in_app_id { get; set; } = string.Empty;
        public string device_id { get; set; } = string.Empty;
    }
}
