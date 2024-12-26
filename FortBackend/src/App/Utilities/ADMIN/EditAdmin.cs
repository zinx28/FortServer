using FortBackend.src.App.SERVER.Root;
using FortLibrary;
using FortLibrary.Dynamics.Dashboard;
using FortLibrary.MongoDB.Modules;
using Newtonsoft.Json;
using System.Text;
using static FortLibrary.DiscordAuth;

namespace FortBackend.src.App.Utilities.ADMIN
{
    public class EditAdminClass
    {
        public static async void Send(string username, AdminInfo oldAdminInfo, AdminDataInfo adminInfo, AdminData adminData)
        {
            string webhookUrl = Saved.Saved.DeserializeConfig.DetectedWebhookUrl;

            if (webhookUrl == "")
            {
                Logger.Error($"Webhook is null", "BanAndWebhook");
            }
            else
            {
                var OldRoleName = oldAdminInfo.Role == AdminDashboardRoles.Moderator ? "Moderator" : "Admin";
                var embed2 = new
                {
                    title = $"Updated {OldRoleName}",
                    footer = new { text = $"Edited by {adminData.AdminUserName}" },
                    fields = new[]
                    {
                        new { name = "DisplayName", value = username, inline = false},
                        new { name = "RoleID", value = adminInfo.Role == AdminDashboardRoles.Moderator ? "Moderator" : "Admin", inline = false },
                    },
                    color = 0x00FFFF
                };

                string jsonPayload2 = JsonConvert.SerializeObject(new { embeds = new[] { embed2 } });

                using (var httpClient = new HttpClient())
                {

                    HttpContent httpContent2 = new StringContent(jsonPayload2, Encoding.UTF8, "application/json");
                    try
                    {
                        HttpResponseMessage response2 = await httpClient.PostAsync(webhookUrl, httpContent2);

                        if (!response2.IsSuccessStatusCode)
                        {
                            Logger.Error($"Failed to send message. Status code: {response2.StatusCode}", "BanAndWebhook");
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.Error($"Error sending request: {ex.Message}", "BanAndWebhook");
                    }
                }
            }
        }
    }
}
