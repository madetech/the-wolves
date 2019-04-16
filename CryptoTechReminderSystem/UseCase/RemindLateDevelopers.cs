using System;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindLateDevelopers
    {
        private IGetLateDevelopers _getLateDevelopers;
        private IRemindDeveloper _remindDeveloper;
        private IClock _clock;

        public RemindLateDevelopers(IGetLateDevelopers getLateDevelopers, IRemindDeveloper remindDeveloper, IClock clock)
        {
            _getLateDevelopers = getLateDevelopers;
            _remindDeveloper = remindDeveloper;
            _clock = clock;
        }

        private bool IsHalfHourInterval()
        {
            return _clock.Now().ToUnixTimeSeconds() % 1800 == 0;
        }

        private bool IsBeforeTwoPm()
        {
            return _clock.Now().Hour < 14;
        }
        public void Execute(RemindLateDevelopersRequest remindLateDevelopersRequest)
        {
            if(IsHalfHourInterval() && IsBeforeTwoPm())
            {
                var lateDevelopers = _getLateDevelopers.Execute();
                foreach (var lateDeveloper in lateDevelopers.Developers)
                {
                    _remindDeveloper.Execute(new RemindDeveloperRequest
                    {
                        Channel = lateDeveloper,
                        Text = remindLateDevelopersRequest.Message
                    });
                }
            }
        }
    }
}