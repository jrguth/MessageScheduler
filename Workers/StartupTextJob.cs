using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MessageScheduler.Domain;
using System.Diagnostics.Tracing;
using Twilio.Exceptions;
using System.Diagnostics;

namespace MessageScheduler.Workers
{
    public class StartupTextJob
    {
        private IConfiguration configuration;
        private ISmsClient smsClient;
        public StartupTextJob(ISmsClient smsClient, IConfiguration configuration)
        {
            this.smsClient = smsClient;
            this.configuration = configuration;
        }
        public void Execute()
        {
            Trace.TraceInformation($"Sending startup text to {configuration["TestSmsPhoneNumber"]}");
            try
            {
                smsClient.SendSmsMessage(configuration["TestSmsPhoneNumber"], "Hello! Message Scheduler is up and running!");
            }
            catch (TwilioException e)
            {
                Trace.TraceError($"Failed to send startup text to {configuration["TestSmsPhoneNumber"]}: {e}");
            }
        }
    }
}
