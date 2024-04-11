namespace FortLibrary.XMPP
{
    public class RoomsData
    {
        public List<MembersData> members = new List<MembersData>();
    }

    public class MembersData
    {
        public string accountId { get; set; } = string.Empty;
    }
}
