namespace FortLibrary.XMPP
{
    public class Pings
    {
        public string sent_to { get; set; } = string.Empty;
        public string sent_by { get; set; } = string.Empty;
        public string sent_at { get; set; } = string.Empty;

        public string expires_at { get; set; } = string.Empty;
        public string time { get; set; } = string.Empty;
        public object meta { get; set; } = new();
    }
}
