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
    public class HarvestGateway : IHarvestDeveloperRetriever, ITimeSheetRetriever
    {
        private readonly HttpClient _client;
        private readonly string _token;
        private readonly string _accountId;
        private readonly string _userAgent;

        private static string ToHarvestApiString(DateTimeOffset date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        
        public HarvestGateway(string address, string token, string accountId, string userAgent)
        {
            _client = new HttpClient { BaseAddress = new Uri(address) };
            _token = token;
            _accountId = accountId;
            _userAgent = userAgent;
        }
        
        private async Task<JObject> GetApiResponse(string address)
        {
            var response = await _client.GetAsync(address);
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        public IList<HarvestDeveloper> RetrieveDevelopers()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            _client.DefaultRequestHeaders.Add("Harvest-Account-Id",_accountId);
            _client.DefaultRequestHeaders.Add("User-Agent",_userAgent);

            var response = GetApiResponse("/api/v2/users");
            
            response.Wait();
            
            var apiResponse = response.Result;
            var users = apiResponse["users"];
           
            return users.Where (user => (bool)user["is_active"] != false && CheckUserRole(user))
                .Select(developer => new HarvestDeveloper()
                    {
                        Id = (int) developer["id"],
                        FirstName = developer["first_name"].ToString(),
                        LastName = developer["last_name"].ToString(),
                        Email = developer["email"].ToString()
                    }
                ).ToList();
        }

        private bool CheckUserRole(JToken user)
        {
            var userRole = user["roles"].Select(role => role.ToString());
            return (userRole.Contains("Software Engineer") ||
                    userRole.Contains("Senior Software Engineer") ||
                    userRole.Contains("Senior Engineer") ||
                    userRole.Contains("Lead Engineer") ||
                    userRole.Contains("Delivery Manager") ||
                    userRole.Contains("SRE") ||
                    userRole.Contains("Consultant") ||
                    userRole.Contains("Delivery Principal")
                   );
        }

        public IList<TimeSheet> RetrieveTimeSheets(DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var address = $"/api/v2/time_entries?from={ToHarvestApiString(dateFrom)}&to={ToHarvestApiString(dateTo)}";
            
            var response = GetApiResponse(address);
            
            response.Wait();
            
            var apiResponse = response.Result;
            var timeSheets = apiResponse["time_entries"];
            return timeSheets.Select(timeSheet => new TimeSheet
                {
                    Id = (int)timeSheet["id"],
                    TimeSheetDate = timeSheet["spent_date"].ToString(),
                    UserId = (int)timeSheet["user"]["id"],
                    Hours = (float)timeSheet["hours"]
                }
            ).ToList(); 
        }
    }
}
