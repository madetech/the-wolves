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
    public class SlackGateway : IMessageSender
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
            var request = JsonConvert.SerializeObject(new PostMessageRequest());
            var content = new StringContent(request, Encoding.UTF8, "application/json");
            var result = PostChatMessage(content).Result;
        }
        
        private async Task<object> PostChatMessage(StringContent content)
        {
            var requestPath = "/api/chat.postMessage";
            var response = await _client.PostAsync(requestPath, content);
            return await response.Content.ReadAsStringAsync();;
        }
    }

    public class PostMessageRequest
    {
        public string channel { get; set; }
        public string text { get; set; }
    }
}