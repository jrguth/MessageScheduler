using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace MessageScheduler.Domain
{
    public class TwilioClient
    {
        private readonly string twilioNumber;
        private readonly TwilioRestClient client;

        public TwilioClient(string accountSid, string authToken, string twilioNumber)
        {
            this.client = new TwilioRestClient(accountSid, authToken);
            this.twilioNumber = twilioNumber;
        }

        public void SendSmsMessage(string phoneNumber, string message)
        {
            var to = new PhoneNumber(phoneNumber);
            MessageResource.Create(
                to: to,
                from: new PhoneNumber(twilioNumber),
                body: message,
                client: client);
        }
    }
}
