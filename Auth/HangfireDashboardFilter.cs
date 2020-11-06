using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace MessageScheduler.Auth
{
    public class HangfireDashboardFilter : IDashboardAuthorizationFilter
    {
        private readonly IRequestAuthStrategy authStrategy;
        public HangfireDashboardFilter(IRequestAuthStrategy authStrategy)
        {
            this.authStrategy = authStrategy;
        }
        public bool Authorize([NotNull] DashboardContext context)
        {
            return authStrategy.IsRequestAuthorized(context.GetHttpContext());
        }
    }
}
