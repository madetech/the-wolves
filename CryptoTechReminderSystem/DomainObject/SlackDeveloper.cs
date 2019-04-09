namespace CryptoTechReminderSystem.DomainObject
{
    public class SlackDeveloper : Developer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool IsBot { get; set; }
    }
}