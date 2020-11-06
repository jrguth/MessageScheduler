using MessageScheduler.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MessageScheduler.Auth
{
    public class NonApiMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IRequestAuthStrategy requestAuthStrategy;
        
        public NonApiMiddleware(RequestDelegate next, IRequestAuthStrategy requestAuthStrategy)
        {
            this.next = next;
            this.requestAuthStrategy = requestAuthStrategy;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                if (!requestAuthStrategy.IsRequestAuthorized(context))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }               
            }
            await next.Invoke(context);
        }
    }
}
