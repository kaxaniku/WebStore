using AutoMapper;
using WebStore.Application.Interfaces.Repositories;
using WebStore.Application.Interfaces.Services;
using WebStore.Domain;

namespace WebStore.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public static event Action<Admin>? AdminRegistered;
    public static event Action<Admin>? AdminUpdated;
    public static event Action<int>? AdminRemoved;

    public AdminService(IUnitOfWork unitOfWork, IEmailService emailService, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        await _unitOfWork.AdminRepository.InsertAsync(dto, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        Admin.SetId(adminEntity, dto.Id);
        await _emailService.SendEmailAsync(adminEntity.Email, "Welcome to KN-Industry-WebStore", "Thank you for registering with us.");
        OnAdminRegistered(adminEntity);
        return adminEntity.Id;
    }

    public async Task ChangePasswordAsync(int adminId, string oldPw, string newPw, CancellationToken cancellationToken)
    {
        var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin == null) throw new KeyNotFoundException();
        var adminEntity = _mapper.Map<Admin>(admin);
        Admin.SetNewPassword(adminEntity, oldPw, newPw);
        admin = _mapper.Map<DTOs.Admin>(adminEntity);

        await _unitOfWork.AdminRepository.UpdateAsync(admin);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        OnAdminUpdated(adminEntity);
    }

    public async Task UpdateUsernameAsync(int adminId, string newUsername, CancellationToken cancellationToken)
    {
        var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin == null) throw new KeyNotFoundException();
        var adminEntity = _mapper.Map<Admin>(admin);
        Admin.UpdateUsername(adminEntity, newUsername);
        admin = _mapper.Map<DTOs.Admin>(adminEntity);

        await _unitOfWork.AdminRepository.UpdateAsync(admin);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        OnAdminUpdated(adminEntity);
    }

    public async Task UpdateEmailAsync(int adminId, string newEmail, CancellationToken cancellationToken)
    {
        var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin == null) throw new KeyNotFoundException();
        var adminEntity = _mapper.Map<Admin>(admin);
        Admin.UpdateEmail(adminEntity, newEmail);
        admin = _mapper.Map<DTOs.Admin>(adminEntity);

        await _unitOfWork.AdminRepository.UpdateAsync(admin);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        OnAdminUpdated(adminEntity);
    }

    public async Task RemoveAdminAsync(int adminId, CancellationToken cancellationToken)
    {
        var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin == null) throw new KeyNotFoundException();
        _unitOfWork.AdminRepository.Delete(admin);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        OnAdminRemoved(adminId);
    }

    private static void OnAdminRegistered(Admin admin)
    {
        AdminRegistered?.Invoke(admin);
    }

    private static void OnAdminUpdated(Admin admin)
    {
        AdminUpdated?.Invoke(admin);
    }

    private static void OnAdminRemoved(int adminId)
    {
        AdminRemoved?.Invoke(adminId);
    }
}