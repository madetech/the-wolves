using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CryptoTechReminderSystem.DomainObject;
using Newtonsoft.Json.Linq;

namespace CryptoTechReminderSystem.Gateway
{
    public class HarvestGateway : IDeveloperRetriever
    {
        private readonly HttpClient _client;
        private string _token;

        public HarvestGateway(string address, string token)
        {
            _client = new HttpClient { BaseAddress = new Uri(address) };
            _token = token;
        }

        public IList<Developer> Retrieve()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            
            var apiResponse = GetUsers().Result;
            var users = apiResponse["users"];
            IList<Developer> developers = new List<Developer>();
            
            foreach (var developer in users)
            {
                developers.Add(new Developer()
                {
                   Id = (int)developer["id"],
                   FirstName = developer["first_name"].ToString(),
                   LastName = developer["last_name"].ToString(),
                   Email = developer["email"].ToString()
                });
            }
            
            return developers;
        }
        
        private async Task<JObject> GetUsers()
        {
            var requestUrl = "/api/v2/users";
            var response = await _client.GetAsync(requestUrl);
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());
            return result;
        }

        public IEnumerable<TimeSheet> RetrieveTimeSheets()
        {
            var response = new List<TimeSheet>();
            response.Add(new TimeSheet()
            {
                user = new User()
                {
                    name = "Bob Incomplete"
                }
            });
            return response;
        }
    }
}