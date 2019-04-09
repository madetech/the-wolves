using System;
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
        [JsonProperty("created_at")] 
        public DateTime CreatedAt { get; set; }
        [JsonProperty("updated_at")] 
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
    }
}