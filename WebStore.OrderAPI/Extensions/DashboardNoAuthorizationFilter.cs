using Hangfire.Dashboard;

namespace WebStore.OrderAPI.Extensions
{
    public class DashboardNoAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
