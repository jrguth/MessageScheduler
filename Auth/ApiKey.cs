using AspNetCore.Authentication.ApiKey;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MessageScheduler.Auth
{
    public class ApiKey : IApiKey
    {
        public string Key { get; set; }

        public string OwnerName { get; set; }

        public IReadOnlyCollection<Claim> Claims { get; set; } = new List<Claim>();

        public IReadOnlyCollection<string> Roles { get; set; } = new List<string> { "User" };
    }
}
