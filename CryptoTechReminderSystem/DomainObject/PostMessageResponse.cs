using System;

namespace CryptoTechReminderSystem.DomainObject
{
    public class PostMessageResponse<TSuccess, TException>
    {
        private TSuccess Success { get; set; }
        private TException Error { get; set; }

        public static PostMessageResponse<TSuccess, TException> OfSuccessful(TSuccess successful)
        {
            return new PostMessageResponse<TSuccess, TException> {Success = successful};
        }

        public static PostMessageResponse<TSuccess, TException> OfError(TException error)
        {
            return new PostMessageResponse<TSuccess, TException> {Error = error};
        }

        public void OnSuccess(Action<TSuccess> action)
        {
            if (Success != null) action(Success);
        }

        public void OnError(Action<TException> action)
        {
            if (Error != null) action(Error);
        }
    }
    public class Success{}
}