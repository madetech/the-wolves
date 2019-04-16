using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class SendReminderSpy : ISendReminder
    {
        public bool Called { private set; get; }
        public int CountCalled{ private set; get; }
        public List<string> Channels{ private set; get; }
        public string Text{ private set; get; }

        public SendReminderSpy()
        {
            Channels = new List<string>();
        }
        
        public void Execute(SendReminderRequest sendReminderRequest)
        {
            Called = true;
            CountCalled++;
            Channels.Add(sendReminderRequest.Channel);
            Text = sendReminderRequest.Text;
        }
    }
}