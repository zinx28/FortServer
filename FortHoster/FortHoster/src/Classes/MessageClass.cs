using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortHoster.src.Classes
{
    public class MessageClass
    {
        public string Message { get; set; } = string.Empty;
        public string ID { get; set; } = string.Empty;
    }

    public class Servers
    {
        public string ID { get; set; } = "";
        // maybe smth else!!?!?! but this is to tell matchmaker server closed
    }
}
