using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoYo_Web_App.Models.Yoyo
{
    public class TestCompletedResponse
    {
        public IEnumerable<AthleteInfo> AthleteInfos { get; set; }
        public bool IsValid { get; set; }
    }
}
