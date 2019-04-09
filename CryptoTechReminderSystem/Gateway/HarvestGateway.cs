using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CryptoTechReminderSystem.DomainObject;
using Newtonsoft.Json.Linq;

namespace CryptoTechReminderSystem.Gateway
{
    public class HarvestGateway : ITimesheetAndDeveloperRetriever
    {
        private readonly HttpClient _client;
        private string _token;

        public HarvestGateway(string address, string token)
        {
            _client = new HttpClient { BaseAddress = new Uri(address) };
            _token = token;
        }
        
        private async Task<JObject> GetApiResponse(string address)
        {
            var response = await _client.GetAsync(address);
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        public IList<HarvestDeveloper> RetrieveDevelopers()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
 
            var apiResponse = GetApiResponse("/api/v2/users").Result;
            var users = apiResponse["users"];
            return users.Select(developer => new HarvestDeveloper
                {
                    Id = (int) developer["id"],
                    FirstName = developer["first_name"].ToString(),
                    LastName = developer["last_name"].ToString(),
                    Email = developer["email"].ToString()
                }
            ).ToList(); 
        }

        public IEnumerable<TimeSheet> RetrieveTimeSheets()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
 
            var apiResponse = GetApiResponse("/api/v2/time_entries").Result;
            var timeSheets = apiResponse["time_entries"];
            return timeSheets.Select(timeSheet => new TimeSheet
                {
                    Id = (int)timeSheet["id"],
                    TimeSheetDate = timeSheet["spent_date"].ToString(),
                    UserId = (int)timeSheet["user"]["id"],
                    Hours = (float)timeSheet["hours"],
                    CreatedAt = DateTime.Parse(timeSheet["created_at"].ToString()),
                    UpdatedAt = DateTime.Parse(timeSheet["updated_at"].ToString())
                }
            ).ToList(); 
        }
    }

    public class HarvestDeveloper : Developer
    {
        public int Id { get; set; }
    }
}