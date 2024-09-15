using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using YoYo_Web_App.Models;

namespace YoYo_Web_App.ViewModels
{
    public class YoyoViewModel
    {
        public IEnumerable<AthleteInfo> AthleteInfos { get; set; }
        public IEnumerable<SelectListItem> LevelShuttles { get; set; }
    }
}
