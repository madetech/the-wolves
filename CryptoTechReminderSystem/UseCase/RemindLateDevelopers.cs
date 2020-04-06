using System;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindLateBillablePeople
    {
        private readonly IGetLateBillablePeople _getLateBillablePeople;
        private readonly ISendReminder _sendReminder;

        public RemindLateBillablePeople(IGetLateBillablePeople getLateBillablePeople, ISendReminder sendReminder)
        {
            _getLateBillablePeople = getLateBillablePeople;
            _sendReminder = sendReminder;
        }
        
        public void Execute(RemindLateBillablePeopleRequest remindLateBillablePeopleRequest)
        {
            var lateBillablePeople = _getLateBillablePeople.Execute();
            
            foreach (var lateDeveloper in lateBillablePeople.BillablePeople)
            {
                _sendReminder.Execute(new SendReminderRequest
                {
                    Channel = lateDeveloper.Id,
                    Text = remindLateBillablePeopleRequest.Message,
                    Email = lateDeveloper.Email
                });
            }
        }
    }
}
