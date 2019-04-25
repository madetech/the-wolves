using System;
using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.Gateway
{
    public interface IMessageSender
    {
        PostMessageResponse<Success, Exception> Send(Message message);
    }
}