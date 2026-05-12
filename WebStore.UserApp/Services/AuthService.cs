using WebStore.UserApp.Interfaces.Repositories;
using WebStore.UserApp.Interfaces.Services;
using WebStore.UserDomain.Entities;

namespace WebStore.UserApp.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ValidateCustomerCredentialsAsync(string username, string password, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var givenPasswordHash = Password.Create(password);
        var customer = await _unitOfWork.CustomerRepository.GetByUsernameAsync(username, cancellationToken);
        if (customer == null) return false;
        return customer.PasswordHash == givenPasswordHash;
    }

    public async Task<bool> ValidateAdminCredentialsAsync(string username, string password, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var givenPasswordHash = Password.Create(password);
        var admin = await _unitOfWork.AdminRepository.GetByUsernameAsync(username, cancellationToken);
        if (admin == null) return false;
        return admin.PasswordHash == givenPasswordHash;
    }

    public async Task<Customer?> CustomerLogin(string username, string password, CancellationToken cancellationToken)
    {
        if (await ValidateCustomerCredentialsAsync(username, password, cancellationToken))
        {
            var customerDto = await _unitOfWork.CustomerRepository.GetByUsernameAsync(username, cancellationToken);
            return Customer.Create(customerDto!.Username, customerDto!.Email, customerDto!.PasswordHash);
        }
        return null;
    }

    public async Task<Admin?> AdminLogin(string username, string password, CancellationToken cancellationToken)
    {
        if (await ValidateAdminCredentialsAsync(username, password, cancellationToken))
        {
            var adminDto = await _unitOfWork.AdminRepository.GetByUsernameAsync(username, cancellationToken);
            return Admin.Create(adminDto!.Username, adminDto!.Email, adminDto!.PasswordHash);
        }
        return null;
    }
}
