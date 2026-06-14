using WebStore.CatalogDomain.Entities;

namespace WebStore.CatalogApp.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Admin?> AdminLogin(string username, string password, CancellationToken cancellationToken);
        Task ChangePasswordAsync(int adminId, string oldPw, string newPw, CancellationToken cancellationToken);
        Task<int> RegisterAdminAsync(int id, string username, string password, CancellationToken cancellationToken);
        Task RemoveAdminAsync(int adminId, CancellationToken cancellationToken);
        Task UpdateUsernameAsync(int adminId, string newUsername, CancellationToken cancellationToken);
        Task<bool> ValidateAdminCredentialsAsync(string username, string password, CancellationToken cancellationToken);
    }
}