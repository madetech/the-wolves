using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.UseCase;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoTechReminderSystem.Gateway
{
    public class SlackGateway : IMessageSender, ISlackDeveloperRetriever
    {
        private readonly HttpClient _client;
        private readonly string _token;

        public SlackGateway(string address, string token)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(address)
            };
            _token = token;
        }

        public void Send(Message message)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            
            var request = JsonConvert.SerializeObject(new PostMessageRequest
            {
                Channel = message.Channel,
                Text = message.Text
            });
            var content = new StringContent(request, Encoding.UTF8, "application/json");
            PostChatMessageAsync(content).Wait();
        }
        
        private async Task<string> PostChatMessageAsync(HttpContent content)
        {
            var response = await _client.PostAsync("/api/chat.postMessage", content);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public IList<SlackDeveloper> RetrieveDevelopers()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var result = GetUsersAsync().Result;
            var users = result["members"];

            return users.Select(developer => new SlackDeveloper
                {
                    Id = developer["id"].ToString(),
                    Email = developer["profile"]["email"].ToString()
                }
            ).ToList();
        }
        
        private async Task<JObject> GetUsersAsync(){
            var response = await _client.GetAsync("/api/users.list");
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }
        
        private class PostMessageRequest
        {
            [JsonProperty("channel")]
            public string Channel { get; set; }
            [JsonProperty("text")]
            public string Text { get; set; }
        }
    }
}
