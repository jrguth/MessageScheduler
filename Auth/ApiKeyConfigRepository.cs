using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.Configuration;

namespace MessageScheduler.Auth
{
    public class ApiKeyConfigRepository : IApiKeyRepository
    {
        private IConfiguration config;
        public ApiKeyConfigRepository (IConfiguration config)
        {
            this.config = config;
        }

        public IApiKey GetApiKey()
        {
            return new ApiKey
            {
                Key = config.GetValue<string>("Authorization"),
                OwnerName = "User"
            };
        }

        public IApiKey GetAdminApiKey()
        {
            return new ApiKey
            {
                Key = config.GetValue<string>("AdminAuthorization"),
                OwnerName = "Administrator",
                Roles = new List<string> { "User", "Administrator" }
            };
        }
    }
}
