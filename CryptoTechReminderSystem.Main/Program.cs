using System;
using System.Collections.Generic;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;

namespace CryptoTechReminderSystem.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var remindUser = new RemindUser(new MessageSender("https://slack.com/", "xoxb-588882280116-591298073303-hfZeBHsJ3bo45DczunWm07Sq"));
            
            IList<string> idsList = new List<string>()
            {
                "UH3DVNTH7", 
                "UH9NYEWSC", 
                "UHC4PEXM2", 
                "UHCABC886", 
                "UHCFC7Z61", 
                "UHDJJA52S"
            };

            foreach (var id in idsList)
            {
                remindUser.Execute(new RemindUserRequest
                {
                    UserId = id
                });
            }
        }
    }
}