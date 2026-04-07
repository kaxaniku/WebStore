using WebStore.Domain;

namespace WebStore.Application.Interfaces.Services
{
    public interface IAdminService
    {
        static abstract event Action<Admin>? AdminRegistered;
        static abstract event Action<int>? AdminRemoved;
        static abstract event Action<Admin>? AdminUpdated;

        Task<IEnumerable<Admin>> GetAllAdminsAsync(CancellationToken cancellationToken);
        Task<Admin> GetAdminByIdAsync(int adminId, CancellationToken cancellationToken);
        Task ChangePasswordAsync(int adminId, string oldPw, string newPw, CancellationToken cancellationToken);
        Task<int> RegisterAdminAsync(string username, string email, string password, CancellationToken cancellationToken);
        Task RemoveAdminAsync(int adminId, CancellationToken cancellationToken);
        Task UpdateEmailAsync(int adminId, string newEmail, CancellationToken cancellationToken);
        Task UpdateUsernameAsync(int adminId, string newUsername, CancellationToken cancellationToken);
    }
}