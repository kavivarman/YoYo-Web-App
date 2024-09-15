using System.Collections.Generic;
using System.Threading.Tasks;
using YoYo_Web_App.ViewModels;

namespace YoYo_Web_App.Models.Yoyo
{
    public interface IYoyoRepository
    {
        Task<YoyoViewModel> GetViewModelAsync();
        Task<IEnumerable<BeepTestData>> GetBeepTestData();
        AthleteInfo UpdateAthleteStatus(int id, ActionType type, LevelShuttleDetail levelShuttleDetail);
        TestCompletedResponse OnTestCompleted(IEnumerable<LevelShuttleDetails> levelShuttleDetails);
        IEnumerable<AthleteInfo> ViewResult();
    }
}
