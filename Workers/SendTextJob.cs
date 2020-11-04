using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;
using MessageScheduler.Data;
using MessageScheduler.Domain;
using Microsoft.Extensions.Configuration;
using Twilio.Exceptions;
using System.Diagnostics;

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
                Trace.TraceInformation($"Attempting to send message '{message}' to user {text.FirstName} {text.LastName} with phone number {text.PhoneNumber} ");
                try
                {
                    smsClient.SendSmsMessage(text.PhoneNumber, message);
                }
                catch (TwilioException e)
                {
                    Trace.TraceError($"Failed to send text with message '{message}' to user {text.FirstName} {text.LastName} with phone number {text.PhoneNumber}: {e}");
                }
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
