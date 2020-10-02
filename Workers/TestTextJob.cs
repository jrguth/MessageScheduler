using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MessageScheduler.Domain;

namespace MessageScheduler.Workers
{
    public class TestTextJob
    {
        private IConfiguration configuration;
        public TestTextJob(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void Execute()
        {
            var twilioClient = new TwilioClient(configuration["TwilioAccountSid"], configuration["TwilioAuthToken"], configuration["TwilioPhoneNumber"]);
            twilioClient.SendSmsMessage(configuration["TestSmsPhoneNumber"], "HI BIB!");
        }
    }
}
