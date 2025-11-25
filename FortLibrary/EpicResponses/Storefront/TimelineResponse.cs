using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.Storefront
{
    public class TimelineResponse
    {
        public TimelineResponseChannels channels { get; set; } = new TimelineResponseChannels();
        public int eventsTimeOffsetHrs { get; set; } = 0;
        public float cacheIntervalMins { get; set; } = 15;
        public string currentTime { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }

    public class TimelineResponseChannels
    {
        [JsonPropertyName("client-matchmaking")]
        public ClientMatchmakingTL ClientMatchmaking { get; set; } = new ClientMatchmakingTL();

        [JsonPropertyName("standalone-store")]
        public ClientMatchmakingTL StandaloneStore { get; set; } = new ClientMatchmakingTL();

        [JsonPropertyName("tk")]
        public ClientMatchmakingTL tk { get; set; } = new ClientMatchmakingTL();

        [JsonPropertyName("community-votes")]
        public ClientMatchmakingTL CommunityVotes { get; set; } = new ClientMatchmakingTL();

        [JsonPropertyName("featured-islands")]
        public ClientMatchmakingTL FeaturedIslands { get; set; } = new ClientMatchmakingTL();

        [JsonPropertyName("client-events")]
        public ClientEventsTL ClientEvents { get; set; } = new ClientEventsTL();
    }

    public class ClientMatchmakingTL
    {
        public List<ClientMatchmakingTL_States> states { get; set; } = new();
        public string cacheExpire { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }

    public class ClientMatchmakingTL_States
    {
        public string validFrom { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public List<object> activeEvents { get; set; } = new();
        public object state { get; set; } = new();
    }

    public class ClientEventsTL
    {
        public List<ClientEventsStates> states { get; set; } = new List<ClientEventsStates>();
        public string cacheExpire { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }

    public class ClientEventsStates
    {
        public string validFrom { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public List<ActiveEventData> activeEvents { get; set; } = new List<ActiveEventData>();
        public ClientEventsStatesState state { get; set; } = new ClientEventsStatesState();
    }

    public class ActiveEventData
    {
        public string eventType { get; set; } = "TestEvent!";
        public string activeUntil { get; set; } = "9999-01-01T00:00:00.000Z";
        public string activeSince { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }

    public class ClientEventsStatesState
    {
        public List<object> activeStorefronts { get; set; }

        public List<object> activeEvents = new();
        public object eventNamedWeights { get; set; } = new();
        public int seasonNumber { get; set; } = 0;
        public string eventPunchCardTemplateId { get; set; } = "";
        public string seasonTemplateId { get; set; }
        public int matchXpBonusPoints { get; set; } = 0;
        public object sectionStoreEnds { get; set; } = new();
        public string rmtPromotion { get; set; } = "";
        public string seasonBegin { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public string  seasonEnd { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public string seasonDisplayedEnd { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public string weeklyStoreEnd { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public string stwEventStoreEnd { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public string stwWeeklyStoreEnd { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public string dailyStoreEnd { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
}