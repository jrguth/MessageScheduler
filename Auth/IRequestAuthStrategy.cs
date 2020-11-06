using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace MessageScheduler.Auth
{
    public interface IRequestAuthStrategy
    {
        bool IsRequestAuthorized(HttpContext context);
    }
}
