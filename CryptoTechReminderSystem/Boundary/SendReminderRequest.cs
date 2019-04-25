namespace CryptoTechReminderSystem.Boundary
{
    public class SendReminderRequest
    {
        public string Channel { get; set; }
        public string Text { get; set; }
        public string Email { get; set; }
    }
}