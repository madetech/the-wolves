using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CryptoTechReminderSystem.DomainObject;
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

        public PostMessageResponse<bool, Exception> Send(Message message)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            
            var request = JsonConvert.SerializeObject(new PostMessageRequest
            {
                Channel = message.Channel,
                Text = message.Text
            });
            var content = new StringContent(request, Encoding.UTF8, "application/json");
            var response = PostChatMessageAsync(content);

            response.Wait();

            if ((bool) response.Result["ok"])
            {
                return PostMessageResponse<bool, Exception>.OfSuccessful(true);
            }

            return PostMessageResponse<bool, Exception>
                .OfError(new Exception(response.Result["error"].ToString()));
        }
        
        public IList<SlackDeveloper> RetrieveDevelopers()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = GetUsersAsync();
            
            response.Wait();
            
            var result = response.Result;
            var users = result["members"];

            return users.Where(user => user["profile"]["email"] != null && 
                                       (bool) user["deleted"] != true &&
                                       (bool) user["is_ultra_restricted"] != true)
            .Select(developer => new SlackDeveloper
                {
                    Id = developer["id"].ToString(),
                    Email = developer["profile"]["email"].ToString()
                }
            ).ToList();
        }
        
        private async Task<JObject> PostChatMessageAsync(HttpContent content)
        {
            var response = await _client.PostAsync("/api/chat.postMessage", content);
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }
        
        private async Task<JObject> GetUsersAsync() {
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

    public class PostMessageResponse<TBool, TException>
    {
        private TBool Success { get; set; }
        private TException Error { get; set; }

        public static PostMessageResponse<TBool, TException> OfSuccessful(TBool successful)
        {
            return new PostMessageResponse<TBool, TException> {Success = successful};
        }

        public static PostMessageResponse<TBool, TException> OfError(TException error)
        {
            return new PostMessageResponse<TBool, TException> {Error = error};
        }

        public void OnSuccess(Action<TBool> action)
        {
            if (Success != null) action(Success);
        }

        public void OnError(Action<TException> action)
        {
            if (Error != null) action(Error);
        }
    }
}
