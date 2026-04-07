using AutoMapper;
using WebStore.Application.Interfaces.Repositories;
using WebStore.Application.Interfaces.Services;
using WebStore.Domain;

namespace WebStore.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public static event Action<Customer>? CustomerRegistered;
    public static event Action<Customer>? CustomerUpdated;
    public static event Action<int>? CustomerRemoved;

    public CustomerService(IUnitOfWork unitOfWork, IEmailService emailService, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken)
    {
        var customers = await _unitOfWork.CustomerRepository.QueryAsync(x => x.Activity.IsActive, cancellationToken);
        return _mapper.Map<IEnumerable<Customer>>(customers);
    }

    public async Task<Customer> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer == null) throw new KeyNotFoundException();
        return _mapper.Map<Customer>(customer);
    }

    public async Task<int> RegisterCustomerAsync(string username, string email, string password, CancellationToken cancellationToken)
    {
        var customerEntity = Customer.Create(username, email, password);
        var dto = _mapper.Map<DTOs.Customer>(customerEntity);

        await _unitOfWork.CustomerRepository.InsertAsync(dto, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        Customer.SetId(customerEntity, dto.Id);
        await _emailService.SendEmailAsync(customerEntity.Email, "Welcome to KN-Industry-WebStore", "Thank you for registering with us.");
        OnCustomerRegistered(customerEntity);
        return customerEntity.Id;
    }

    public async Task ChangePasswordAsync(int customerId, string oldPw, string newPw, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer == null) throw new KeyNotFoundException();
        var customerEntity = _mapper.Map<Customer>(customer);
        Customer.SetNewPassword(customerEntity, oldPw, newPw);
        customer = _mapper.Map<DTOs.Customer>(customerEntity);

        await _unitOfWork.CustomerRepository.UpdateAsync(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        OnCustomerUpdated(customerEntity);
    }

    public async Task UpdateUsernameAsync(int customerId, string newUsername, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer == null) throw new KeyNotFoundException();
        var customerEntity = _mapper.Map<Customer>(customer);
        Customer.UpdateUsername(customerEntity, newUsername);
        customer = _mapper.Map<DTOs.Customer>(customerEntity);

        await _unitOfWork.CustomerRepository.UpdateAsync(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        OnCustomerUpdated(customerEntity);
    }

    public async Task UpdateEmailAsync(int customerId, string newEmail, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer == null) throw new KeyNotFoundException();
        var customerEntity = _mapper.Map<Customer>(customer);
        Customer.UpdateEmail(customerEntity, newEmail);
        customer = _mapper.Map<DTOs.Customer>(customerEntity);

        await _unitOfWork.CustomerRepository.UpdateAsync(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        OnCustomerUpdated(customerEntity);
    }

    public async Task RemoveCustomerAsync(int customerId, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer == null) throw new KeyNotFoundException();
        _unitOfWork.CustomerRepository.Delete(customer);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        OnCustomerRemoved(customerId);
    }

    private static void OnCustomerRegistered(Customer customer)
    {
        CustomerRegistered?.Invoke(customer);
    }

    private static void OnCustomerUpdated(Customer customer)
    {
        CustomerUpdated?.Invoke(customer);
    }

    private static void OnCustomerRemoved(int customerId)
    {
        CustomerRemoved?.Invoke(customerId);
    }
}