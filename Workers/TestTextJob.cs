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
        private ISmsClient smsClient;
        public TestTextJob(ISmsClient smsClient, IConfiguration configuration)
        {
            this.smsClient = smsClient;
            this.configuration = configuration;
        }
        public void Execute()
        {
            smsClient.SendSmsMessage(configuration["TestSmsPhoneNumber"], "Hello! This is an automated message test");
        }
    }
}
