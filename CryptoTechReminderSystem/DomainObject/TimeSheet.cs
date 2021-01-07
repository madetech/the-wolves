using System.Collections.Generic;
using Newtonsoft.Json;

namespace CryptoTechReminderSystem.DomainObject
{
    public class TimeSheet
    {
        [JsonProperty("id")] 
        public int Id { get; set; }

        [JsonProperty("spent_date")] 
        public string TimeSheetDate { get; set; }

        [JsonProperty("hours")] 
        public double Hours { get; set; }

        public int UserId { get; set; }
        
        [JsonProperty("is_closed")]
        public bool IsClosed { get; set; }

        [JsonProperty("project_manager_ids")]
        public List<int> ProjectManagerIds { get; set; }

    }
}
