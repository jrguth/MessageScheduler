using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageScheduler.Data;
using MessageScheduler.Domain;
using Microsoft.Extensions.Configuration;

namespace MessageScheduler.Workers
{
    public class SendTextJob
    {
        private const string message = "Hello! This is an automated message test";
        private IScheduledTextRepository repo;
        private ISmsClient smsClient;

        public SendTextJob(IScheduledTextRepository repo, ISmsClient smsClient)
        {
            this.repo = repo;
            this.smsClient = smsClient;
        }

        public void Execute()
        {
            foreach (ScheduledText text in GetTextsToSend())
            {
                smsClient.SendSmsMessage(text.PhoneNumber, message);
            }
        }

        private IEnumerable<ScheduledText> GetTextsToSend()
        {
            return repo
                .GetScheduledTexts()
                .Where(text => DateTime.UtcNow.Minute == text.DateUTC.Minute);
        }
    }
}
