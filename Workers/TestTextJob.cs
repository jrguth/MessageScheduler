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
    public class TestTextJob
    {
        private IConfiguration configuration;
        private ISmsClient smsClient;
        public TestTextJob(ISmsClient smsClient, IConfiguration configuration)
        {
            this.smsClient = smsClient;
            this.configuration = configuration;
        }
        public void Execute()
        {
            Trace.TraceInformation($"Sending test text to {configuration["TestSmsPhoneNumber"]}");
            try
            {
                smsClient.SendSmsMessage(configuration["TestSmsPhoneNumber"], "Hello! This is an automated message test");
            }
            catch (TwilioException e)
            {
                Trace.TraceError($"Failed to send test text to {configuration["TestSmsPhoneNumber"]}: {e}");
            }
        }
    }
}
