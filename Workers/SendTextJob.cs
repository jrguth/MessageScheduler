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
        private const string message = "Hi BIB!";
        private IScheduledTextRepository repo;
        private IConfiguration configuration;

        public SendTextJob(IScheduledTextRepository repo, IConfiguration configuration)
        {
            this.repo = repo;
            this.configuration = configuration;
        }

        public void Execute()
        {
            var twilioClient = new TwilioClient(configuration["TwilioAccountSid"], configuration["TwilioAuthToken"], configuration["TwilioPhoneNumber"]);
            
            foreach (ScheduledText text in GetTextsToSend())
            {
                twilioClient.SendSmsMessage(text.PhoneNumber, message);
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
