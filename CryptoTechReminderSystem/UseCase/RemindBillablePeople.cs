using System;
using CryptoTechReminderSystem.Boundary;

namespace CryptoTechReminderSystem.UseCase
{
    public class RemindBillablePeople
    {
        private readonly IGetLateBillablePeople _getLateBillablePeople;
        private readonly ISendReminder _sendReminder;

        public RemindBillablePeople(IGetLateBillablePeople getLateBillablePeople, ISendReminder sendReminder)
        {
            _getLateBillablePeople = getLateBillablePeople;
            _sendReminder = sendReminder;
        }
        
        public void Execute(RemindBillablePeopleRequest remindLateBillablePeopleRequest)
        {
            var lateBillablePeople = _getLateBillablePeople.Execute();
            
            foreach (var lateBillablePerson in lateBillablePeople.BillablePeople)
            {
                _sendReminder.Execute(new SendReminderRequest
                {
                    Channel = lateBillablePerson.Id,
                    Text = remindLateBillablePeopleRequest.Message,
                    Email = lateBillablePerson.Email
                });
            }
        }
    }
}
