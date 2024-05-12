using FortBackend.src.App.Utilities.Constants;
using FortLibrary.Dynamics;
using FortLibrary.MongoDB.Module;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.Helpers.BattlepassManagement
{
    public class LevelUpdater
    {
        public static async Task<(SeasonClass FoundSeason, bool NeedItems)> Init(int Season, SeasonClass FoundSeason, bool NeedItems)
        {//
         // var SeasonXPFolder = Path.Combine(PathConstants.BaseDir, $"Json\\Season\\Season{Season}\\SeasonXP.json");
         ////  var SeasonBattleStarsFolder = Path.Combine(PathConstants.BaseDir, $"Json\\Season\\Season{Season}\\SeasonBP.json");

            List<SeasonBP> seasonBPStars = BattlepassManager.SeasonBattleStarsItems.FirstOrDefault(e => e.Key == (FoundSeason.SeasonNumber)).Value;
            List<SeasonXP> seasonXp = BattlepassManager.SeasonBattlePassXPItems.FirstOrDefault(e => e.Key == FoundSeason.SeasonNumber).Value;

            if(seasonBPStars == null || seasonBPStars.Count <= 0)
            {
                if (Season > 1 && Season <= 10)
                {
                    if ((FoundSeason.SeasonNumber + 1) > 100)
                    {

                    }else
                    {
                        Logger.Error("Missing Season Battlestars data on " + Season); // shouldn't happen?
                    }
                }

            }
          
            if (seasonXp != null && seasonXp.Count > 0)
            {
                foreach (SeasonXP item in seasonXp)
                {
                    if (item.Level == FoundSeason.Level)
                    {
                        if (FoundSeason.SeasonXP > item.XpToNextLevel)
                        {
                            FoundSeason.SeasonXP -= item.XpToNextLevel;
                            FoundSeason.Level += 1;

                            // i need to set for season 1!!!
                            if (Season <= 1)
                            {
                                FoundSeason.BookLevel = FoundSeason.Level;
                            }

                            if (seasonBPStars != null && seasonBPStars.Count > 0)
                            {
                                SeasonBP SeasonBpObject = seasonBPStars.FirstOrDefault(e => e.Level == FoundSeason.Level)!;

                                if (SeasonBpObject != null)
                                {
                                    FoundSeason.BookXP += SeasonBpObject.BattleStars;
                                }
                            }

                            // Battle stars are implemented again in chapter 2 season 7
                            if (Season > 10 && Season < 17)
                            {
                                NeedItems = true;
                                FoundSeason.BookLevel = FoundSeason.Level;
                            }

                            if (Season >= 17)
                            {
                                // Implement newer battlestar system...
                                Logger.Error("LEVEL UP, STARS ARE NOT SUPPORTED ON SEASON 17 AND ABOVE", "ClientQuestLogiN");
                            }
                        }

                        // Done
                        if (FoundSeason.BookXP < item.XpToNextLevel) break;
                    }
                }



                // They are added back during chapter 2 season 7 but they don't auto claim
                if (Season > 1 && Season <= 10)
                {
                    while (FoundSeason.BookXP >= 10)
                    {
                        FoundSeason.BookXP -= 10;
                        FoundSeason.BookLevel += 1;
                        NeedItems = true;
                        // require to give the items 
                    }
                }
            }

            return (FoundSeason, NeedItems);
        }
    }
}
