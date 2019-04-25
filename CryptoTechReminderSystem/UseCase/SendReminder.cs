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
            
            result.OnSuccess(f => Console.WriteLine($"{sendReminderRequest.Email} was sent a reminder."));
            result.OnError(error => Console.WriteLine(
                $"!Failed to send message to {sendReminderRequest.Email} with error: {error.Message}"));
        }
    }
}