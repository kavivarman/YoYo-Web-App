using YoYo_Web_App.Models.Yoyo;

namespace YoYo_Web_App.Models
{
    public class AthleteInfo
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public LevelShuttleDetail Warned { get; set; }
        public LevelShuttleDetail Completed { get; set; }
    }
}
