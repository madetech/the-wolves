using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.DomainObject;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace CryptoTechReminderSystem.Test.UseCase
{
    public class RemindDeveloperTests
    {
        private const string Text = "Please make sure your timesheet is submitted today by 13:30.";

        private class SlackGatewaySpy : IMessageSender
        {
            public Message Message;
        
            public void Send(Message message)
            {
                Message = message;
            }
        }
       
        [Test]
        public void CanRemindDeveloper()
        {
            var spy = new SlackGatewaySpy();
            var remindDeveloper = new SendReminder(spy);
            
            remindDeveloper.Execute(new SendReminderRequest
            {
                Channel = "U120123D",
                Text = Text
            });
            
            spy.Message.Channel.Should().Be("U120123D");
        }
        
        [Test]
        public void CanRemindDeveloper2()
        {
            var spy = new SlackGatewaySpy();
            var remindDeveloper = new SendReminder(spy);

            remindDeveloper.Execute(new SendReminderRequest
            {
                Channel = "U87219AW",
                Text = Text
            });
            
            spy.Message.Channel.Should().Be("U87219AW");
            spy.Message.Text.Should().Be(Text);
        }
    }
}