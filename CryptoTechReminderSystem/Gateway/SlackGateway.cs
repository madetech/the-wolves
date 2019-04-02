using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.UseCase;
using Newtonsoft.Json;

namespace CryptoTechReminderSystem.Gateway
{
    public class SlackGateway : ISlackGateway
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
            var result = PostChatMessage(content).Result;
            Console.WriteLine(result);
        }
        
        private async Task<object> PostChatMessage(StringContent content)
        {
            var requestPath = "/api/chat.postMessage";
            var response = await _client.PostAsync(requestPath, content);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }

    public class PostMessageRequest
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}