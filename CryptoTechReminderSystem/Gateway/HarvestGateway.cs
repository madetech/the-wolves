using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CryptoTechReminderSystem.Gateway
{
    public class HarvestGateway
    {
        private readonly HttpClient _client;
        private string _token;

        public HarvestGateway(string address, string token)
        {
            _client = new HttpClient { BaseAddress = new Uri(address) };
            _token = token;
        }

        public string Retrieve()
        {
            return GetUsers().Result;
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