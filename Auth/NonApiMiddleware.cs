using AutoMapper.Configuration;
using MessageScheduler.Configuration;
using MessageScheduler.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Diagnostics;
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
                Trace.TraceInformation("Received non-api request at path: {0}", context.Request.Path);
                if (!requestAuthStrategy.IsRequestAuthorized(context))
                {
                    Trace.TraceWarning("Request is not authorized");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }               
            }
            await next.Invoke(context);
        }
    }
}
