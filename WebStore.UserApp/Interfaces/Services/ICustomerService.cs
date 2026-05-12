using WebStore.UserDomain.Entities;

namespace WebStore.UserApp.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken);
        Task<Customer> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken);
        Task ChangePasswordAsync(int customerId, string oldPw, string newPw, CancellationToken cancellationToken);
        Task<int> RegisterCustomerAsync(string username, string email, string password, CancellationToken cancellationToken);
        Task RemoveCustomerAsync(int customerId, CancellationToken cancellationToken);
        Task UpdateEmailAsync(int customerId, string newEmail, CancellationToken cancellationToken);
        Task UpdateUsernameAsync(int customerId, string newUsername, CancellationToken cancellationToken);
    }
}