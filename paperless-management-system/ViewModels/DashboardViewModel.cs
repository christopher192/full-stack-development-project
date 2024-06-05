using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WD_ERECORD_CORE.Data;

namespace WD_ERECORD_CORE.ViewModels
{
    public class DashboardViewModel
    {
        public CalendarList MyCalender { get; set; }
        public ApplicationUser User { get; set; }
        public List<AnnouncementList> Announcement { get; set; }
    }
    
    public class DailySubmission
    {
        [JsonProperty(PropertyName = "Series")]
        public int Series { get; set; }

        [JsonProperty(PropertyName = "Labels")]
        public string Labels { get; set; }
    }

    public class WeeklySubmission
    {
        [JsonProperty(PropertyName = "Series")]
        public int Series { get; set; }

        [JsonProperty(PropertyName = "Labels")]
        public string Labels { get; set; }
    }

    public class MonthlySubmission
    {
        [JsonProperty(PropertyName = "Series")]
        public int Series { get; set; }

        [JsonProperty(PropertyName = "Labels")]
        public string Labels { get; set; }
    }
    public class AnnualSubmission
    {
        public string FormName { get; set; }
        public int Month { get; set; }
        public int Total { get; set; }
    }

    public class AnnualSubmissionDataTransformation
    {
        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "data")]
        public List<int> data { get; set; }
    }
}
