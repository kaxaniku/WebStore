using WebStore.UserDomain.Entities;

namespace WebStore.UserApp.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Admin?> AdminLogin(string username, string password, CancellationToken cancellationToken);
        Task<Customer?> CustomerLogin(string username, string password, CancellationToken cancellationToken);
        Task<bool> ValidateAdminCredentialsAsync(string username, string password, CancellationToken cancellationToken);
        Task<bool> ValidateCustomerCredentialsAsync(string username, string password, CancellationToken cancellationToken);
    }
}