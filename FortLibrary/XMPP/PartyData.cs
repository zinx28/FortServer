using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.XMPP
{
    public class PartyData
    {
        public string partyId { get; set; } = string.Empty;
        public PartyDataPayload payload { get; set; } = new PartyDataPayload();
    }

    public class PartyDataPayload
    {
        public int Rev { get; set; } = 0;
        public PartyDataPayloadAttrs Attrs { get; set; } = new PartyDataPayloadAttrs();
    }

    public class PartyDataPayloadAttrs
    {
        public string PartyState_s { get; set; } = string.Empty;
        public string MatchmakingInfoString_s { get; set; } = "TEST";
    }
}
