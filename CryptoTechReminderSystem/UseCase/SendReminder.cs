using System;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;

namespace CryptoTechReminderSystem.UseCase
{
    public class SendReminder : ISendReminder
    {
        private readonly IMessageSender _slackGateway;

        public SendReminder(IMessageSender slackGateway)
        {
            _slackGateway = slackGateway;
        }

        public void Execute(SendReminderRequest sendReminderRequest)
        {
            var result = _slackGateway.Send(new Message
            {
                Channel = sendReminderRequest.Channel,
                Text = sendReminderRequest.Text
            });

            var address = sendReminderRequest.Email ?? sendReminderRequest.Channel;
            
            result.OnSuccess(success => Console.WriteLine($"{address} was sent a reminder."));
            result.OnError(error => Console.WriteLine(
                $"!Failed to send message to {address} with error: {error.Message}"));
        }
    }
}