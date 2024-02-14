using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Storefront;
using FortBackend.src.App.Utilities.Shop.Helpers.Class;
using FortBackend.src.App.Utilities.Shop.Helpers.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FortBackend.src.App.Routes.APIS.Storefront
{
    [ApiController]
    [Route("fortnite/api/storefront/v2/catalog")]
    public class CatalogController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Catalog> Catalog()
        {
            Response.ContentType = "application/json";
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/json/shop/shop.json");
                string json = System.IO.File.ReadAllText(filePath);

                if(string.IsNullOrEmpty(json))
                {
                    Logger.Error("Catalog is null -> weird issue");
                    return Ok(new Catalog());
                }
                ShopJson shopData = JsonConvert.DeserializeObject<ShopJson>(json);

                if(shopData == null)
                {
                    Logger.Error("shopData is null -> weird issue");
                    return Ok(new Catalog());
                }

                Catalog ShopObject = new Catalog
                {
                    refreshIntervalHrs = 24,
                    dailyPurchaseHrs = 24,
                    expiration = $"{shopData.expiration}",
                };

                foreach (var WeeklyItems in shopData.ShopItems.Weekly)
                {

                }

                return Ok(ShopObject);
            }
            catch (Exception ex)
            {
                Logger.Error($"CatalogShop -> {ex.Message}");
            }

            return Ok(new Catalog());
        }
    }
}
