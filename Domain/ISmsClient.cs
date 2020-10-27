namespace MessageScheduler.Domain
{
    public interface ISmsClient
    {
        void SendSmsMessage(string phoneNumber, string message);
    }
}