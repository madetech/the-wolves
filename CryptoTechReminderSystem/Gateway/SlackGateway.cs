using System;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.UseCase;
using System.Net.Http;
using System.Threading.Tasks;

namespace CryptoTechReminderSystem.Gateway
{
    public class MessageSender : IMessageSender
    {
        private readonly HttpClient _client;
        
        public MessageSender(string httpLocalhost)
        {
            _client = new HttpClient {BaseAddress = new Uri(httpLocalhost)};
        }

        public void Send(Message message)
        {
            var content = new StringContent(message.UserId);
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