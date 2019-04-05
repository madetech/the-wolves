using System;

namespace CryptoTechReminderSystem.DomainObject
{
    public class TimeSheet
    {
        public int id { get; set; }
        public string spent_date { get; set; }
        public User user { get; set; }
        public double hours { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
    
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}