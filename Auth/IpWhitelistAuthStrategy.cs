using MessageScheduler.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace MessageScheduler.Auth
{
    public class IpWhitelistAuthStrategy : IRequestAuthStrategy
    {
        private readonly IpSafeListConfig safeListConfig;
        public IpWhitelistAuthStrategy(IOptions<IpSafeListConfig> options)
        {
            safeListConfig = options.Value;
        }
        public bool IsRequestAuthorized(HttpContext context)
        {
            IPAddress remoteIp = context.Connection.RemoteIpAddress;
            Trace.TraceInformation("Checking if request from IP {0} is whitelisted in {1}", remoteIp.ToString(), string.Join(";", safeListConfig.IpAddresses));
            return safeListConfig.IpAddresses
                .Where(ip => IPAddress.Parse(ip).Equals(remoteIp))
                .Any();
        }
    }
}
