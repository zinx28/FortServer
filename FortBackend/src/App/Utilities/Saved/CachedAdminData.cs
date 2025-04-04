using FortLibrary.Dynamics.Dashboard;

namespace FortBackend.src.App.Utilities.ADMIN
{
    public class CachedAdminData
    {
        public List<AdminData> Data { get; set; } = new List<AdminData>();
    }

    public class AdminData
    {
        public string AccessToken { get; set; } = "";
        public bool IsForcedAdmin { get; set; } = false;
        public string AdminUserEmail { get; set; } = "";
        public string AdminUserName { get; set; } = "Admin";
        public int RoleId { get; set; } = AdminDashboardRoles.Moderator;
        public bool bIsSetup { get; set; } = false;
    }
}
