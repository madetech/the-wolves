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
    }
}
