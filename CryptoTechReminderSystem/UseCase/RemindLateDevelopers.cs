using System;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindLateDevelopers
    {
        private IGetLateDevelopers _getLateDevelopers;
        private IRemindDeveloper _remindDeveloper;

        public RemindLateDevelopers(IGetLateDevelopers getLateDevelopers, IRemindDeveloper remindDeveloper, IClock clock)
        {
            _getLateDevelopers = getLateDevelopers;
            _remindDeveloper = remindDeveloper;
        }

        public void Execute(RemindLateDevelopersRequest remindLateDevelopersRequest)
        {
            var lateDevelopers = _getLateDevelopers.Execute();
            foreach (var lateDeveloper in lateDevelopers.Developers)
            {
                _remindDeveloper.Execute(new RemindDeveloperRequest()
                {
                    Channel = "lad",
                    Text = "lad"
                
                });
            }
            
        }
    }
}