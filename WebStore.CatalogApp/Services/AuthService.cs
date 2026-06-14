using MapsterMapper;
using WebStore.CatalogApp.Interfaces.Repositories;
using WebStore.CatalogApp.Interfaces.Services;
using WebStore.CatalogDomain.Entities;

namespace WebStore.CatalogApp.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuthService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

    public async Task<Admin?> AdminLogin(string username, string password, CancellationToken cancellationToken)
    {
        if (await ValidateAdminCredentialsAsync(username, password, cancellationToken))
        {
            var adminDto = await _unitOfWork.AdminRepository.GetByUsernameAsync(username, cancellationToken);
            var admin = Admin.Create(adminDto!.Username, adminDto!.PasswordHash);
            Admin.SetId(admin, adminDto.Id);
            return admin;
        }
        return null;
    }

    public async Task<int> RegisterAdminAsync(int id, string username, string password, CancellationToken cancellationToken)
    {
        var adminEntity = Admin.Create(username, password);
        Admin.SetId(adminEntity, id);
        var dto = _mapper.Map<DTOs.Admin>(adminEntity);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            await _unitOfWork.AdminRepository.InsertAsync(dto, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return adminEntity.Id;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw new Exception("An error occurred while registering the admin.", ex);
        }
    }

    public async Task ChangePasswordAsync(int adminId, string oldPw, string newPw, CancellationToken cancellationToken)
    {
        var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin == null) throw new KeyNotFoundException();
        var adminEntity = _mapper.Map<Admin>(admin);
        Admin.SetNewPassword(adminEntity, oldPw, newPw);
        _mapper.Map(adminEntity, admin);

        await _unitOfWork.AdminRepository.UpdateAsync(admin);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateUsernameAsync(int adminId, string newUsername, CancellationToken cancellationToken)
    {
        var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin == null) throw new KeyNotFoundException();
        var adminEntity = _mapper.Map<Admin>(admin);
        Admin.UpdateUsername(adminEntity, newUsername);
        _mapper.Map(adminEntity, admin);

        await _unitOfWork.AdminRepository.UpdateAsync(admin);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAdminAsync(int adminId, CancellationToken cancellationToken)
    {
        var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin == null) throw new KeyNotFoundException();
        _unitOfWork.AdminRepository.Delete(admin);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
