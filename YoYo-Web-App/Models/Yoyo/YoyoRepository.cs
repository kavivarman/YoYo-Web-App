using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using YoYo_Web_App.Mapper;
using YoYo_Web_App.Models.AppConfig;
using YoYo_Web_App.ViewModels;

namespace YoYo_Web_App.Models.Yoyo
{
    /// <summary>
    /// The YoyoRepository contains 
    /// logic for all yoyo controller action methods
    /// </summary>
    public class YoyoRepository : IYoyoRepository
    {
        private readonly IAppConfig _appConfig;
        private readonly IMapper _mapper;
        private IEnumerable<BeepTestData> beepTestDatas;
        private IEnumerable<AthleteInfo> athleteInfos;

        /// <summary>
        /// The Repository constructor.
        /// </summary>
        /// <param name="appConfig">Contains application configuration</param>
        /// <param name="mapper">Contains mapper configuration</param>
        public YoyoRepository(IAppConfig appConfig, IMapper mapper)
        {
            _appConfig = appConfig;
            _mapper = mapper;
        }

        /// <summary>
        /// This method contains logic to get athletes and beep test json template data to load yoyo page .
        /// </summary>
        /// <returns>YoyoViewModel</returns>
        public async Task<YoyoViewModel> GetViewModelAsync()
        {
            beepTestDatas = await GetBeepTestData();
            athleteInfos = await GetAthletesInfo();
            if (beepTestDatas != null && athleteInfos != null)
                return _mapper.Map<IEnumerable<AthleteInfo>, YoyoViewModel>(athleteInfos)
                                .Map(GenerateLevelShuttleList(beepTestDatas), _mapper);
            return null;
        }

        /// <summary>
        /// This method reads beep test data from given json file. 
        /// </summary>
        /// <returns>IEnumerable of BeepTestData</returns>
        public async Task<IEnumerable<BeepTestData>> GetBeepTestData()
        {
            if (beepTestDatas != null && beepTestDatas.Count() > 0)
            {
                return beepTestDatas;
            }
            else
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), _appConfig?.BeepTestFilePath ?? "wwwroot\\beepTestFile\\fitnessrating_beeptest.json");
                if (!File.Exists(filePath))
                    return null;
                var fs = File.OpenRead(filePath);
                return await JsonSerializer.DeserializeAsync<IEnumerable<BeepTestData>>(fs);
            }
        }
        /// <summary>
        /// This method contains logic to read athletes info from  athletes_info json file.
        /// </summary>
        /// <returns>IEnumerable of AthleteInfo</returns>
        private async Task<IEnumerable<AthleteInfo>> GetAthletesInfo()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), _appConfig?.AthletesFilePath ?? "wwwroot\\beepTestFile\\athletes_info.json");
            if (!File.Exists(filePath))
                return null;
            var fs = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<IEnumerable<AthleteInfo>>(fs);
        }

        /// <summary>
        /// This method contains logic to generate Level, Shuttle for dropdown. 
        /// </summary>
        /// <param name="beepTestDatas">List of Beep Test Data</param>
        /// <returns>IEnumerable of SelectListItem</returns>
        private IEnumerable<SelectListItem> GenerateLevelShuttleList(IEnumerable<BeepTestData> beepTestDatas)
        {
            return beepTestDatas.Select(a => new SelectListItem() { Value = $"{a?.SpeedLevel}-{a?.ShuttleNo}", Text = $"{a?.SpeedLevel}-{a?.ShuttleNo}" });
        }
        /// <summary>
        /// This method contains logic to save warned/stopped athletes
        /// </summary>
        /// <param name="id">Unique athlete ID</param>
        /// <param name="type">Type of action Warned/Completed(Stopped)()</param>
        /// <param name="levelShuttleDetail">Contains Level Shuttle Detail</param>
        /// <returns>successfully updated AthleteInfo</returns>
        public AthleteInfo UpdateAthleteStatus(int id, ActionType type, LevelShuttleDetail levelShuttleDetail)
        {
            var athleteInfo = athleteInfos.FirstOrDefault(a => a?.Id == id);
            if (athleteInfo != null)
            {
                switch (type)
                {
                    case ActionType.Completed:
                        {
                            athleteInfo.Completed = levelShuttleDetail;
                            break;
                        }
                    case ActionType.Warned:
                        {
                            athleteInfo.Warned = levelShuttleDetail;
                            break;
                        }
                }
                return athleteInfo;
            }
            return null;
        }
        /// <summary>
        /// This method contains logic to save list of athletes level, shuttle details.
        /// </summary>
        /// <param name="levelShuttleDetails">Contains list of athletes id with stopped(completed) level, shuttle details</param>
        /// <returns>successfully saved list of athletes info</returns>
        public TestCompletedResponse OnTestCompleted(IEnumerable<LevelShuttleDetails> levelShuttleDetails)
        {
            var testCompletedResponse = new TestCompletedResponse()
            {
                IsValid = false
            };
            if (!ValidateLevelShuttleDetails(levelShuttleDetails))
                return testCompletedResponse;

            var UpdatedAthleteInfo = new List<AthleteInfo>();
            foreach (var levelShuttleDetail in levelShuttleDetails)
            {
                var athleteInfo = athleteInfos.FirstOrDefault(a => a?.Id == levelShuttleDetail?.Id);
                if (athleteInfo?.Completed == null)
                {
                    athleteInfo.Completed = levelShuttleDetail.LevelShuttleDetail;
                    UpdatedAthleteInfo.Add(athleteInfo);
                }
            }
            testCompletedResponse = new TestCompletedResponse()
            {
                IsValid = true,
                AthleteInfos = UpdatedAthleteInfo
            };
            return testCompletedResponse;
        }
        /// <summary>
        /// This method contains logic to validate all the athletes ids are valid or not.
        /// </summary>
        /// <param name="levelShuttleDetails">List of level shuttle details of athletes</param>
        /// <returns></returns>
        private bool ValidateLevelShuttleDetails(IEnumerable<LevelShuttleDetails> levelShuttleDetails)
        {
            var valid = true;
            foreach (var levelShuttleDetail in levelShuttleDetails)
            {
                var athleteInfo = athleteInfos.FirstOrDefault(a => a?.Id == levelShuttleDetail.Id);
                if (athleteInfo == null)
                {
                    valid = false;
                    break;
                }
            }
            return valid;
        }
        /// <summary>
        /// This method gets Athletes Info
        /// </summary>
        /// <returns>list of athleteInfo</returns>
        public IEnumerable<AthleteInfo> ViewResult()
        {
            return athleteInfos ?? null;
        }
    }
}
