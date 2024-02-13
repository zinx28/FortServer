namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Errors
{
    public class BaseError
    {
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string[] messageVars { get; set; }
        public long numericErrorCode { get; set; }
        public string originatingService { get; set; }
        public string intent { get; set; }
        public string error_description { get; set; }
        public string error { get; set; }
    }
}
