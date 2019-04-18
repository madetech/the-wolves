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
        private readonly IClock _clock;
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
                Environment.GetEnvironmentVariable("HARVEST_ACCOUNT_ID")
            );
            _clock = new Clock();
            _getLateDevelopers = new GetLateDevelopers(slackGateway, harvestGateway, harvestGateway, _clock);
            _sendReminder = new SendReminder(slackGateway);

            CreateSchedule();
        }

        private void ResetSchedule()
        {
            // Stops scheduled jobs then sets them up again for next week
            JobManager.RemoveAllJobs();
            CreateSchedule();
        }
        
        private void CreateSchedule()
        {
            // Creates schedule that starts and stops schedule jobs
            JobManager.AddJob(ScheduleJobs, s => s.ToRunEvery(1).Weeks().On(DayOfWeek.Friday).At(10,30));
            JobManager.AddJob(ResetSchedule, s => s.ToRunEvery(1).Weeks().On(DayOfWeek.Friday).At(13,45));
        }

        private void ScheduleJobs()
        {
            // Do all of your actual job scheduling here
            JobManager.AddJob(RemindLateDevelopersJob, s => s.ToRunEvery(30).Minutes());
            JobManager.AddJob(ShameLateDevelopersJob, s => s.ToRunOnceAt(13, 30));
        }
        
        private void RemindLateDevelopersJob()
        {
            var remindLateDevelopers = new RemindLateDevelopers(_getLateDevelopers, _sendReminder, _clock);
            remindLateDevelopers.Execute(
                new RemindLateDevelopersRequest
                {
                    Message = "Please make sure your timesheet is submitted by 13:30 on Friday."
                }
            );
        }
        
        private void ShameLateDevelopersJob()
        {
            var shameLateDevelopers = new ShameLateDevelopers(_getLateDevelopers, _sendReminder, _clock);
            shameLateDevelopers.Execute(
                new ShameLateDevelopersRequest
                {
                    Message = "These are the people yet to submit timesheets:",
                    Channel = "CGHBW6YGM"
                }
            );
        }
    }
}
