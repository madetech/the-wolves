using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Test.TestDouble
{
    public class RemindDeveloperSpy : IRemindDeveloper
    {
        public bool Called { private set; get; }
        public int CountCalled{ private set; get; }
        public List<string> Channels{ private set; get; }
        public string Text{ private set; get; }

        public RemindDeveloperSpy()
        {
            Channels = new List<string>();
        }
        
        public void Execute(RemindDeveloperRequest remindDeveloperRequest)
        {
            Called = true;
            CountCalled++;
            Channels.Add(remindDeveloperRequest.Channel);
            Text = remindDeveloperRequest.Text;
        }
    }
}