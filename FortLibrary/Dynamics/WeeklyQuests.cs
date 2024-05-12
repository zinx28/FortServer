using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.Dynamics
{
    public class WeeklyQuestsJson
    {
        public string BundleId { get; set; } = string.Empty;
        public string BundleSchedule { get; set; } = string.Empty;

        public WeeklyQuestJsonBundleRequired BundleRequired { get; set; } = new WeeklyQuestJsonBundleRequired();

        public List<WeeklyObjects> FreeBundleObject { get; set; } = new List<WeeklyObjects>();
        public List<WeeklyObjects> PaidBundleObject { get; set; } = new List<WeeklyObjects>();
    }

    public class WeeklyQuestJsonBundleRequired
    {
        public int RequiredLevel { get; set; } = 1;
    }

    public class WeeklyObjects
    {
        public string templateId { get; set; } = string.Empty;
        public WeeklyRewards Rewards { get; set; } = new WeeklyRewards();
        public List<WeeklyObjectsObjectives> Objectives { get; set; } = new List<WeeklyObjectsObjectives>();
    }

    public class WeeklyObjectsObjectives
    {
        public string BackendName { get; set; } = string.Empty;
        public int Count { get; set; } = 0;
        public int Stage { get; set; } = -1;
    }

    public class WeeklyRewards
    {
        public int BattleStars { get; set; } = 0;
        public List<WeeklyRewardsQuest> Quest { get; set; } = new List<WeeklyRewardsQuest>();
    }

    public class WeeklyRewardsQuest
    {
        public string PrimaryAssetName { get; set; } = string.Empty;
        public int Quantity { get; set; } = 0;
    }
}
