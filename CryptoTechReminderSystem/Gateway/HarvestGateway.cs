using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CryptoTechReminderSystem.DomainObject;
using Newtonsoft.Json;

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
            
            var developer = JsonConvert.DeserializeObject<Developer>(GetUsers().Result);

            IList<Developer> response = new List<Developer>()
            {
                developer
            };
            
            return response;
        }
        
        private async Task<string> GetUsers()
        {
            var requestUrl = "/api/v2/users";
            var response = await _client.GetAsync(requestUrl);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}