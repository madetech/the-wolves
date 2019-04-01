using System;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.UseCase;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CryptoTechReminderSystem.Gateway
{
    public class MessageSender : IMessageSender
    {
        public class PostMessageRequest
        {
            public string channel{ get; set; }
            public string text { get; set; }
            public string icon_emoji { get; set; }
            public bool as_user { get; set; }
            
        }
        
        private readonly HttpClient _client;
        private string _token;

        public MessageSender(string address, string token)
        {
            _client = new HttpClient {BaseAddress = new Uri(address)};
            _token = token;
        }

        public void Send(Message message)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);


            var request = JsonConvert.SerializeObject(new PostMessageRequest
            {
                channel = message.UserId,
                text = "Please make sure your timesheet is submitted by 13:30 on Friday."
            });
            var content = new StringContent(request, Encoding.UTF8,
                "application/json");
            var result = PostChatMessage(content).Result;
            Console.WriteLine(result);
        }

        private async Task<object> PostChatMessage(StringContent content)
        {
            var requestUrl = "/api/chat.postMessage";
            var response = await _client.PostAsync(requestUrl, content);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}