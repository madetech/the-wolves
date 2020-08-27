using System;
using System.Collections.Generic;
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
            try
            {
                DotEnv.Config(false);
                JobManager.Initialize(new ReminderRegistry());
                SpinWait.SpinUntil(() => false);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}.");
            }
        }
    }

    public class ReminderRegistry : Registry
    {
        private readonly GetLateBillablePeople _getLateBillablePeople;
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
                Environment.GetEnvironmentVariable("HARVEST_BILLABLE_ROLES")
            );

            var clock = new Clock();
            _getLateBillablePeople = new GetLateBillablePeople(slackGateway, harvestGateway, harvestGateway, clock);
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
            if (!IsLastDayOfTheMonthFridayOrWeekend())
            {
                JobManager.AddJob(ScheduleJobs, s => s.ToRunEvery(0).Months().OnTheLastDay().At(10, 25));
                JobManager.AddJob(ResetSchedule, s => s.ToRunEvery(0).Months().OnTheLastDay().At(13, 45));
            }
            JobManager.AddJob(ScheduleJobs, s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Friday).At(10, 25));
            JobManager.AddJob(ResetSchedule, s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Friday).At(13, 45));
        }

        private void ScheduleJobs()
        {
            JobManager.AddJob(RemindLateBillablePeopleJob, s => s.ToRunOnceAt(10, 30).AndEvery(30).Minutes());
            JobManager.AddJob(ListLateBillablePeopleJob, s => s.ToRunOnceAt(13, 30));
        }

        private void RemindLateBillablePeopleJob()
        {
            var remindLateBillablePeople = new RemindLateBillablePeople(_getLateBillablePeople, _sendReminder);
            remindLateBillablePeople.Execute(
                new RemindLateBillablePeopleRequest
                {
                    Message = Environment.GetEnvironmentVariable("SLACK_REMINDER_MESSAGE")
                }
            );
        }

        private void ListLateBillablePeopleJob()
        {
            var listLateBillablePeople = new ListLateBillablePeople(_getLateBillablePeople, _sendReminder);
            listLateBillablePeople.Execute(
                new ListLateBillablePeopleRequest
                {
                    LateBillablePeopleMessage = Environment.GetEnvironmentVariable("SLACK_LATE_BILLABLE_PEOPLE_MESSAGE").Replace(@"\n", "\n"),
                    NoLateBillablePeopleMessage = Environment.GetEnvironmentVariable("SLACK_NO_LATE_BILLABLE_PEOPLE_MESSAGE"),
                    Channel = Environment.GetEnvironmentVariable("SLACK_CHANNEL_ID")
                }
            );
        }

        private bool IsLastDayOfTheMonthFridayOrWeekend()
        {
            var lastDayOfTheMonth = DateTime.DaysInMonth(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month);

            var lastDayOfTheMonthDayOfWeek = new DateTimeOffset(
                new DateTime(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, lastDayOfTheMonth)
            ).DayOfWeek;

            return new List<DayOfWeek>
            {
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            }.Contains(lastDayOfTheMonthDayOfWeek);
        }
    }
}
