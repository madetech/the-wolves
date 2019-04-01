using System;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var remindUser = new RemindUser(new MessageSender("https://slack.com/", "xoxb-588882280116-591298073303-AAirGS4weSAifFKyrT17CLvw"));
            
            remindUser.Execute(new RemindUserRequest
            {
                UserId = "UHC4PEXM2"
            });
        }
    }
}