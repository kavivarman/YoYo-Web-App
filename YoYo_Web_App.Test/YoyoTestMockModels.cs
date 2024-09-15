using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using YoYo_Web_App.Models;
using YoYo_Web_App.Models.AppConfig;
using YoYo_Web_App.Models.Yoyo;
using YoYo_Web_App.ViewModels;

namespace YoYo_Web_App.Test
{
    public static class YoyoTestMockModels
    {
        public static AthleteInfo GetAthleteInfoModel()
        {
            return new AthleteInfo()
            {
                Id = 1,
                FullName = "Cristiano Ronaldo",
                Warned = null,
                Completed = new LevelShuttleDetail() { Level = "1", Shuttle = "1" }
            };
        }
        public static IEnumerable<LevelShuttleDetails> GetListOfLevelShuttleDetails()
        {
            return new List<LevelShuttleDetails>()
            {
                new LevelShuttleDetails()
                {
                    LevelShuttleDetail = new LevelShuttleDetail(){ Level ="1", Shuttle ="1"},
                    Id = 1
                },
                new LevelShuttleDetails()
                {
                    LevelShuttleDetail = new LevelShuttleDetail(){ Level ="2", Shuttle ="2"},
                    Id = 2
                }
            };
        }
        public static IEnumerable<LevelShuttleDetails> GetListOfInvalidLevelShuttleDetails()
        {
            return new List<LevelShuttleDetails>()
            {
                new LevelShuttleDetails()
                {
                    LevelShuttleDetail = new LevelShuttleDetail(){ Level ="1", Shuttle ="1"},
                    Id = 1
                },
                new LevelShuttleDetails()
                {
                    LevelShuttleDetail = new LevelShuttleDetail(){ Level ="2", Shuttle ="2"},
                    Id =11
                }
            };
        }
        public static TestCompletedResponse MockTestCompletedResponse()
        {
            return new TestCompletedResponse()
            {
                IsValid = true,
                AthleteInfos = new List<AthleteInfo>()
                {
                    new AthleteInfo() {
                    Id = 1,
                    FullName = "Cristiano Ronaldo",
                    Warned = null,
                    Completed = new LevelShuttleDetail() { Level = "1", Shuttle = "1" }
                    },
                    new AthleteInfo() {
                    Id = 2,
                    FullName = "Lionel Messi",
                    Warned = null,
                    Completed = new LevelShuttleDetail() { Level = "1", Shuttle = "1" }
                    }
                }
            };
        }
        public static IEnumerable<BeepTestData> GetBeepTestModel()
        {
            return new List<BeepTestData>()
            {
                new BeepTestData()
                {
                    AccumulatedShuttleDistance = "40",
                    ApproxVo2Max="36.74",
                    CommulativeTime="00:24",
                    LevelTime="1",
                    ShuttleNo="5",
                    Speed="10",
                    SpeedLevel="14.4",
                    StartTime="00:00"
                },
                new BeepTestData()
                {
                    AccumulatedShuttleDistance = "80",
                    ApproxVo2Max="36.74",
                    CommulativeTime="00:24",
                    LevelTime="1",
                    ShuttleNo="5",
                    Speed="10",
                    SpeedLevel="14.4",
                    StartTime="00:00"
                }
            };
        }
        public static YoyoViewModel GetYoyoViewModel()
        {
            return new YoyoViewModel()
            {
                AthleteInfos = new List<AthleteInfo>()
                {
                     new AthleteInfo()
                     {
                         Completed = null,
                         FullName = "Cristiano Ronaldo",
                         Id=1,
                         Warned=null
                     },
                     new AthleteInfo()
                     {
                         Completed = null,
                         FullName = "Lionel Messi",
                         Id=2,
                         Warned=null
                     }
                 },
                LevelShuttles = new List<SelectListItem>()
                {
                    new SelectListItem()
                    {
                       Text="1-1",
                       Value="1-1"
                    },
                    new SelectListItem()
                    {
                       Text="2-2",
                       Value="2-2"
                    }
                }
            };
        }
        public static AppConfig GetAppConfig()
        {
            return new AppConfig()
            {
                AthletesFilePath = "beepTestFile\\athletes_info.json",
                BeepTestFilePath = "beepTestFile\\fitnessrating_beeptest.json"
            };
        }
    }
}
