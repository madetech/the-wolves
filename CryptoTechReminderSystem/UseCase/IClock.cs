using System;

namespace CryptoTechReminderSystem.UseCase
{
    public interface IClock
    {
        DateTimeOffset Now();
    }
}