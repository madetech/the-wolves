namespace CryptoTechReminderSystem.Boundary
{
    public class ListLateBillablePeopleRequest
    {
        public string LateBillablePeopleMessage { get; set; }
        public string NoLateBillablePeopleMessage { get; set; }
        public string Channel { get; set; }
    }
}
