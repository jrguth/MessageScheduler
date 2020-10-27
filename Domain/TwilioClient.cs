using MessageScheduler.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace MessageScheduler.Domain
{
    public class TwilioClient : ISmsClient
    {
        private readonly TwilioRestClient client;
        private TwilioConfig config;

        public TwilioClient(IOptions<TwilioConfig> options)
        {
            config = options.Value;
            client = new TwilioRestClient(config.AccountSid, config.AuthToken);
        }

        public void SendSmsMessage(string phoneNumber, string message)
        {
            var to = new PhoneNumber(phoneNumber);
            MessageResource.Create(
                to: to,
                from: new PhoneNumber(config.PhoneNumber),
                body: message,
                client: client);
        }
    }
}
