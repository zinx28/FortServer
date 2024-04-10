namespace FortLibrary.EpicResponses.Friends
{
    //"RawSquadAssignments":[{"memberId":"c2092fc3-8f1b-4d70-a036-3b2461e62a1a","absoluteMemberIdx":0}]}
    
    public class RawSquadAssignmentsWrapper
    {
        public List<RawSquadAssignment> RawSquadAssignments { get; set; } = new List<RawSquadAssignment>();
    }
    public class RawSquadAssignment
    {
        public string memberId { get; set; } = string.Empty;
        public int absoluteMemberIdx { get; set; }
    }
}
