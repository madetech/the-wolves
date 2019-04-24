using System;

namespace CryptoTechReminderSystem.DomainObject
{
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