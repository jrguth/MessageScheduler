using MessageScheduler.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
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
            if (remoteIp.IsIPv4MappedToIPv6)
            {
                remoteIp = remoteIp.MapToIPv4();
            }
            return
                (from address in safeListConfig.IpAddresses
                 where !string.IsNullOrEmpty(address) && IPAddress.Parse(address).GetAddressBytes().SequenceEqual(remoteIp.GetAddressBytes())
                 select address).Any();
        }
    }
}
