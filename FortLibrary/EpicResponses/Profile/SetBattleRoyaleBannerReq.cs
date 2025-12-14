namespace FortLibrary.EpicResponses.Profile
{
    public class SetBattleRoyaleBannerReq
    {
        public string homebaseBannerIconId { get; set; } = string.Empty;
        public string homebaseBannerColorId { get; set; } = string.Empty;
    }

    public class SetCosmeticLockerBannerReq
    {
        public string lockerItem { get; set; } = string.Empty;
        public string bannerIconTemplateName { get; set; } = string.Empty;
        public string bannerColorTemplateName { get; set; } = string.Empty;
    }

    public class SetCosmeticLockerNameReq
    {
        public string lockerItem { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
    }

}
