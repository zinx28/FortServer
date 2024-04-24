using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.FortniteServices.Content
{
    public class LoginMessage
    {
        public string _title { get; set; } = "LoginMessage";
        public LoginMessageObject loginmessage { get; set; } = new LoginMessageObject();
        public string _activeDate { get; set; } = "2017-07-19T13:14:04.490Z";
        public string lastModified { get; set; } = "2018-03-15T07:10:22.222Z";
        public string _locale { get; set; } = "en-US";
        public string _templateName { get; set; } = "FortniteGameMOTD";
        public string[] _suggestedPrefetch { get; set; } = new string[0];
    }

    public class LoginMessageObject
    {
        public string _type { get; set; } = "CommonUI Simple Message";
        public LoginMessageContent message { get; set; } = new LoginMessageContent();
      
    }

    public class LoginMessageContent
    {
        public string _type { get; set; } = "CommonUI Simple Message Base";
        public string title { get; set; } = "FortBackend";
        public string body { get; set; } = "FortBackend News";
    }
}
