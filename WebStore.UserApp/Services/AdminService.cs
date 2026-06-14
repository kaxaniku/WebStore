using MapsterMapper;
using MassTransit;
using WebStore.Contracts.User.Admin;
using WebStore.UserApp.Interfaces.Repositories;
using WebStore.UserApp.Interfaces.Services;
using WebStore.UserDomain.Entities;

namespace WebStore.UserApp.Services;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public AdminService(IUnitOfWork unitOfWork, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<IEnumerable<Admin>> GetAllAdminsAsync(CancellationToken cancellationToken)
    {
        var admin = await _unitOfWork.AdminRepository.QueryAsync(x => x.Activity.IsActive, cancellationToken);
        return _mapper.Map<IEnumerable<Admin>>(admin);
    }

    public async Task<Admin> GetAdminByIdAsync(int adminId, CancellationToken cancellationToken)
    {
        var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin == null) throw new KeyNotFoundException();
        return _mapper.Map<Admin>(admin);
    }

    public async Task<int> RegisterAdminAsync(string username, string email, string password, CancellationToken cancellationToken)
    {
        var adminEntity = Admin.Create(username, email, password);
        var dto = _mapper.Map<DTOs.Admin>(adminEntity);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            await _unitOfWork.AdminRepository.InsertAsync(dto, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _publishEndpoint.Publish(new AdminRegistered(dto.Id, dto.Username, dto.Email, password));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            Admin.SetId(adminEntity, dto.Id);
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
        await _publishEndpoint.Publish(new AdminUpdated(admin.Id, oldPw: oldPw, newPw: newPw));
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
        await _publishEndpoint.Publish(new AdminUpdated(admin.Id, admin.Username));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateEmailAsync(int adminId, string newEmail, CancellationToken cancellationToken)
    {
        var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin == null) throw new KeyNotFoundException();
        var adminEntity = _mapper.Map<Admin>(admin);
        Admin.UpdateEmail(adminEntity, newEmail);
        _mapper.Map(adminEntity, admin);

        await _unitOfWork.AdminRepository.UpdateAsync(admin);
        await _publishEndpoint.Publish(new AdminUpdated(admin.Id, admin.Email));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAdminAsync(int adminId, CancellationToken cancellationToken)
    {
        var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin == null) throw new KeyNotFoundException();
        _unitOfWork.AdminRepository.Delete(admin);

        await _publishEndpoint.Publish(new AdminRemoved(admin.Id));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}