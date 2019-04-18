using System;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class ClockStub : IClock
    {
        private readonly DateTimeOffset _currentDateTime;

        public ClockStub(DateTimeOffset dateTime)
        {
            _currentDateTime = dateTime;
        }

        public DateTimeOffset Now()
        {
            return _currentDateTime;
        }
    }
}