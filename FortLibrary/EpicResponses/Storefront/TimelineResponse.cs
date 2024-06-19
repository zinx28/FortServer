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
        public int cacheIntervalMins { get; set; } = 15;
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
        public object[] states { get; set; } = new object[] { };
        public string cacheExpire { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
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
        public object[] activeStorefronts { get; set; }
        public object eventNamedWeights { get; set; }
        public int seasonNumber { get; set; } = 0;
        public string seasonTemplateId { get; set; }
        public int matchXpBonusPoints { get; set; } = 0;
        public string seasonBegin { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public string  seasonEnd { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public string seasonDisplayedEnd { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public string weeklyStoreEnd { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public string stwEventStoreEnd { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public string stwWeeklyStoreEnd { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        public string dailyStoreEnd { get; set; } = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
}