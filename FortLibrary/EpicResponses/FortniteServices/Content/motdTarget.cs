using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.FortniteServices.Content
{
    // chapter 2 news (pretty sure this is only for higher chapter 2 builds and versions above)
    // https://github.com/Krowe-moh/FortniteEndpointsDocumentation/blob/main/EpicGames/PRMDialogService/FNBR-MOTD/MOTD.md?plain=1
    // never planning to change this file again
    public class motdTarget
    {
        public string contentType { get; set; } = "collection";
        public string contentId { get; set; } = "motd-default-collection";
        public string tcId { get; set; } = "f111d6e2-384d-4761-9ba6-c8b4930f5d36";
        public List<contentItems> contentItems { get; set; } = new();
        public MetaData metadata { get; set; } = new();
    }

    public class contentItems
    {
        public string contentType { get; set; } = "content-item";
        public string contentId { get; set; } = "1b707fac-57b1-468c-8248-6773bca2858a";
        public string tcId { get; set; } = "b79859d8-b4e3-4f1f-8bd2-ea1ce564c945";
        public contentFieldsObject contentFields { get; set; } = new();
        public string contentSchemaName { get; set; } = "DiscoveryMotd";

    }

    public class contentFieldsObject
    {
        public string body { get; set; } = "FortBackend is the cutest backend :3";
        public string buttonTextOverride { get; set; } = "Play Now";
        //public string category { get; set; } = "set_br_playlists";
        public string entryType { get; set; } = "Website";  // Text, WebSite
        public ImageClass image { get; set; } = new();
        //public string islandCode { get; set; } = "playlist_defaultSolo";
        public string tabTitleOverride { get; set; } = ":3";
        public ImageClass tileImage { get; set; } = new()
        {
            width = 1024,
            height = 512,
        };

        public string title { get; set; } = ":3";
        public bool videoAutoplay { get; set; } = false;
        public bool videoStreamingEnabled { get; set; } = false;
        // public string videoUID { get; set; } = "xfrwDSOkjtfKpTCwHg";
        // public string videoVideoString { get; set; } = "StarWars_EngagementEvent_GameplayTrailer";
        public string websiteButtonTex { get; set; } = "Github";
        public string websiteURL { get; set; } = "https://github.com/zinx28/FortServer";
    }

    public class ImageClass
    {
        public int width = 1920;
        public int height = 1080;
        public string url = "https://cdn2.unrealengine.com/ch4s2-lobbyupdate-4-20-2022-lifted-copy-3840x2160-d3a138f5f9e7.jpg";
    }

    // juyst gonna predefine this
    public class MetaData
    {
        public string contentType { get; set; } = "content-item";
        public string contentId { get; set; } = "2b89f387-cc8b-473a-8fa3-87891c38570b";
        public string tcId { get; set; } = "031237c8-9639-4f01-99d2-81081bf2b159";

        public MDContentFields contentFields { get; set; } = new();
        public string contentSchemaName { get; set; } = "Metadata";
    }

    public class MDContentFields { public bool autoOpen { get; set; } }
}
