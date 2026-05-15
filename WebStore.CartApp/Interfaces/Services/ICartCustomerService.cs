namespace WebStore.CartApp.Interfaces.Services
{
    public interface ICartCustomerService
    {
        Task<int> RegisterCartCustomerAsync(int id, string username, CancellationToken cancellationToken);
        Task RemoveCartCustomerAsync(int customerId, CancellationToken cancellationToken);
        Task UpdateUsernameAsync(int customerId, string newUsername, CancellationToken cancellationToken);
    }
}