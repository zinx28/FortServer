namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Fortnite
{
    public class CloudstorageFile
    {
        public string uniqueFilename { get; set; } = string.Empty;
        public string filename { get; set; } = string.Empty;
        public string hash { get; set; } = string.Empty;
        public string hash256 { get; set; } = "973124FFC4A03E66D6A4458E587D5D6146F71FC57F359C8D516E0B12A50AB0D9";
        public long length { get; set; }
        public string contentType { get; set; } = "application/octet-stream";
        public DateTime uploaded { get; set; }
        public string storageType { get; set; } = string.Empty;
        public object storageIds { get; set; }
        public string accountId { get; set; } = string.Empty;
        public bool doNotCache { get; set; }
    }
}
