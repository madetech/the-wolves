using System;
using CryptoTechReminderSystem.DomainObject;

namespace CryptoTechReminderSystem.Gateway
{
    public interface IMessageSender
    {
        PostMessageResponse<bool, Exception> Send(Message message);
    }
}