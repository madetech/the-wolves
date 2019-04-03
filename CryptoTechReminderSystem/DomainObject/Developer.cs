using Newtonsoft.Json;

namespace CryptoTechReminderSystem.DomainObject
{
    public class Developer
    {
        [JsonProperty("id")] 
        public int Id { get; set; }
        [JsonProperty("first_name")] 
        public string FirstName { get; set; }
        [JsonProperty("last_name")] 
        public string LastName { get; set; }
        [JsonProperty("email")] 
        public string Email { get; set; }
        [JsonProperty("hours")]
        public int Hours { get; set; }
    }
}