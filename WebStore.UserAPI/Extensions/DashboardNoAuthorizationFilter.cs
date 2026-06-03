using Hangfire.Dashboard;

namespace WebStore.UserAPI.Extensions
{
    public class DashboardNoAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
