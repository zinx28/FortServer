using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.FortniteServices.Content
{
    using FortLibrary.Dynamics;
    using System.Text.Json.Serialization;

    public class MnemonicC
    {
        [JsonPropertyName("namespace")]
        public string @namespace { get; set; } = "fn";

        [JsonPropertyName("mnemonic")]
        public string mnemonic { get; set; }

        [JsonPropertyName("linkType")]
        public string linkType { get; set; } = "BR:Playlist";

        [JsonPropertyName("active")]
        public bool active { get; set; } = true;

        [JsonPropertyName("disabled")]
        public bool disabled { get; set; } = false;

        [JsonPropertyName("version")]
        public int version { get; set; } = 1;

        [JsonPropertyName("moderationStatus")]
        public string moderationStatus { get; set; } = "Unmoderated";

        [JsonPropertyName("accountId")]
        public string accountId { get; set; } = "epic";

        [JsonPropertyName("creatorName")]
        public string creatorName { get; set; } = "Epic";

        [JsonPropertyName("descriptionTags")]
        public string[] descriptionTags { get; set; } = new string[] { };

        [JsonPropertyName("discoveryIntent")]
        public string discoveryIntent { get; set; } = "PUBLIC";

        [JsonPropertyName("metadata")]
        public Metadata_MC metadata { get; set; } = new();
    }

    public class Metadata_MC
    {
        [JsonPropertyName("product_tag")]
        public string product_tag { get; set; } = "Product.FortBackend"; // this shouldnt be used, "Product.BR"
        [JsonPropertyName("image_url")]
        public string image_url { get; set; } = "https://cdn2.unrealengine.com/solo-1920x1080-1920x1080-bc0a5455ce20.jpg";

        [JsonPropertyName("locale")]
        public string locale { get; set; } = "en";

        [JsonPropertyName("title")]
        public string title { get; set; } = "";

        [JsonPropertyName("alt_title")]
        public Languages alt_title { get; set; } = new();

        [JsonPropertyName("matchmaking")]
        public Matchmaking matchmaking { get; set; } = new();

        [JsonPropertyName("tagline")]
        public string tagline { get; set; } = "";

        [JsonPropertyName("introduction")]
        public string introduction { get; set; } = "";
    }

    public class Matchmaking
    {
        [JsonPropertyName("override_playlist")]
        public string override_playlist { get; set; } = "";
    }

}
