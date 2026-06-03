using Hangfire.Dashboard;

namespace WebStore.CatalogAPI.Extensions
{
    public class DashboardNoAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
