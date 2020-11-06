using AspNetCore.Authentication.ApiKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageScheduler.Auth
{
    public interface IApiKeyRepository
    {
        IApiKey GetApiKey();
    }
}
