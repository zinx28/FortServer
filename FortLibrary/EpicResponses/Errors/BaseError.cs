using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace FortLibrary.EpicResponses.Errors
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseError : Exception
    {
        [JsonProperty]
        public string errorCode { get; set; } = string.Empty;

        [JsonProperty]
        public string errorMessage { get; set; } = string.Empty;

        [JsonProperty]
        public List<string> messageVars { get; set; } = new List<string>();

        [JsonProperty]
        public long numericErrorCode { get; set; }

        [JsonProperty]
        public string originatingService { get; set; } = string.Empty;

        [JsonProperty]
        public string intent { get; set; } = string.Empty;

        [JsonProperty]
        public string error_description { get; set; } = string.Empty;

        [JsonProperty]
        public string error { get; set; } = string.Empty;

        public static BaseError FromBaseError(BaseError ex) 
        {
            return new BaseError
            {
                errorCode = ex.errorCode,
                errorMessage = ex.errorMessage,
                messageVars = ex.messageVars,
                numericErrorCode = ex.numericErrorCode,
                originatingService = ex.originatingService,
                intent = ex.intent,
                error_description = ex.error_description,
                error = ex.error
            };
        }
    }
}
