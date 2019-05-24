using System;
using System.Threading;
using CryptoTechReminderSystem.Boundary;
using CryptoTechReminderSystem.Gateway;
using CryptoTechReminderSystem.UseCase;
using dotenv.net;
using FluentScheduler;

namespace CryptoTechReminderSystem.Main
{
    class Clock : IClock
    {
        public DateTimeOffset Now()
        {
            return DateTimeOffset.Now;
        }
    }

    public class Program : Registry
    {
        private static void Main()
        {    
            DotEnv.Config(false);
            JobManager.Initialize(new ReminderRegistry());
            SpinWait.SpinUntil(() => false);
        }
    }

    public class ReminderRegistry : Registry
    {
        private readonly GetLateDevelopers _getLateDevelopers;
        private readonly SendReminder _sendReminder;

        public ReminderRegistry()
        {
            var slackGateway = new SlackGateway(
                "https://slack.com/",
                Environment.GetEnvironmentVariable("SLACK_TOKEN")
            );
            var harvestGateway = new HarvestGateway(
                "https://api.harvestapp.com/",
                Environment.GetEnvironmentVariable("HARVEST_TOKEN"),
                Environment.GetEnvironmentVariable("HARVEST_ACCOUNT_ID"),
                Environment.GetEnvironmentVariable("HARVEST_USER_AGENT"),
                Environment.GetEnvironmentVariable("HARVEST_DEVELOPER_ROLES")
            );
            
            var clock = new Clock();
            _getLateDevelopers = new GetLateDevelopers(slackGateway, harvestGateway, harvestGateway, clock);
            _sendReminder = new SendReminder(slackGateway);

            CreateSchedule();
        }

        private void ResetSchedule()
        {
            JobManager.RemoveAllJobs();
            CreateSchedule();
        }
        
        private void CreateSchedule()
        {
            JobManager.AddJob(ScheduleJobs, s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Friday).At(10,25));
            JobManager.AddJob(ResetSchedule, s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Friday).At(13,45));
        }

        private void ScheduleJobs()
        {
            JobManager.AddJob(RemindLateDevelopersJob, s => s.ToRunOnceAt(10, 30).AndEvery(30).Minutes());
            JobManager.AddJob(ShameLateDevelopersJob, s => s.ToRunOnceAt(13, 30));
        }
        
        private void RemindLateDevelopersJob()
        {
            var remindLateDevelopers = new RemindLateDevelopers(_getLateDevelopers, _sendReminder);
            remindLateDevelopers.Execute(
                new RemindLateDevelopersRequest
                {
                    Message = Environment.GetEnvironmentVariable("SLACK_REMINDER_MESSAGE")
                }
            );
        }
        
        private void ShameLateDevelopersJob()
        {
            var shameLateDevelopers = new ShameLateDevelopers(_getLateDevelopers, _sendReminder);
            shameLateDevelopers.Execute(
                new ShameLateDevelopersRequest
                {
                    Message = Environment.GetEnvironmentVariable("SLACK_SHAME_MESSAGE").Replace(@"\n", "\n"),
                    Channel = Environment.GetEnvironmentVariable("SLACK_CHANNEL_ID")
                }
            );
        }
    }
}
