using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageScheduler.Configuration
{
    public class IpSafeListConfig
    {
        private string IpString { get; set; }
        public IReadOnlyList<string> IpAddresses => IpString
            .Split(';')
            .Where(ip => !string.IsNullOrEmpty(ip))
            .ToList();
    }
}
