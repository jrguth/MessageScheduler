using AspNetCore.Authentication.ApiKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageScheduler.Auth
{
    public class ApiKeyProvider : IApiKeyProvider
    {
        private IApiKeyRepository keyRepo;
        public ApiKeyProvider(IApiKeyRepository keyRepo)
        {
            this.keyRepo = keyRepo;
        }

        public Task<IApiKey> ProvideAsync(string key)
        {
            IApiKey retKey = keyRepo.GetApiKey();
            if (retKey.Key.Equals(key))
            {
                return Task.FromResult(retKey);
            }
            retKey = keyRepo.GetAdminApiKey();
            return retKey.Key.Equals(key)
                ? Task.FromResult(retKey)
                : Task.FromResult<IApiKey>(null);
        }
    }
}
