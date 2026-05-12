using MapsterMapper;
using MassTransit;
using WebStore.Contracts.User.Customer;
using WebStore.UserApp.Interfaces.Repositories;
using WebStore.UserApp.Interfaces.Services;
using WebStore.UserDomain.Entities;

namespace WebStore.UserApp.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
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

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            //await CreateCartForCustomer(dto.Id, cancellationToken);
            await _unitOfWork.CustomerRepository.InsertAsync(dto, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _publishEndpoint.Publish(new CustomerRegistered(dto.Id, dto.Username, dto.Email));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            Customer.SetId(customerEntity, dto.Id);
            await _unitOfWork.CommitAsync(cancellationToken);
            return customerEntity.Id;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw new Exception("An error occurred while registering the customer.", ex);
        }
    }

    public async Task ChangePasswordAsync(int customerId, string oldPw, string newPw, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer == null) throw new KeyNotFoundException();
        var customerEntity = _mapper.Map<Customer>(customer);
        Customer.SetNewPassword(customerEntity, oldPw, newPw);
        _mapper.Map(customerEntity, customer);

        await _unitOfWork.CustomerRepository.UpdateAsync(customer);
        await _publishEndpoint.Publish(new CustomerUpdated(customer.Id, customer.Username, customer.Email));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateUsernameAsync(int customerId, string newUsername, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer == null) throw new KeyNotFoundException();
        var customerEntity = _mapper.Map<Customer>(customer);
        Customer.UpdateUsername(customerEntity, newUsername);
        _mapper.Map(customerEntity, customer);

        await _unitOfWork.CustomerRepository.UpdateAsync(customer);
        await _publishEndpoint.Publish(new CustomerUpdated(customer.Id, customer.Username, customer.Email));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateEmailAsync(int customerId, string newEmail, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer == null) throw new KeyNotFoundException();
        var customerEntity = _mapper.Map<Customer>(customer);
        Customer.UpdateEmail(customerEntity, newEmail);
        _mapper.Map(customerEntity, customer);

        await _unitOfWork.CustomerRepository.UpdateAsync(customer);
        await _publishEndpoint.Publish(new CustomerUpdated(customer.Id, customer.Username, customer.Email));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveCustomerAsync(int customerId, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, cancellationToken)
                ?? throw new KeyNotFoundException("Customer not found");
        //var cart = await _unitOfWork.CartRepository.GetByIdAsync(customerId, cancellationToken)
        //        ?? throw new KeyNotFoundException("Cart not found");

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _unitOfWork.CustomerRepository.Delete(customer);
            //_unitOfWork.CartRepository.Delete(cart);

            await _publishEndpoint.Publish(new CustomerRemoved(customer.Id));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw new Exception("An error occurred while removing the customer.", ex);
        }
    }

    //private async Task CreateCartForCustomer(int customerId, CancellationToken ct)
    //{
    //    var customerDto = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, ct)
    //            ?? throw new KeyNotFoundException("Customer not found");

    //    var customerEntity = _mapper.Map<Customer>(customerDto);
    //    var newCartEntity = Cart.Create(customerEntity);

    //    var newCartDto = _mapper.Map<DTOs.Cart>(newCartEntity);
    //    await _unitOfWork.CartRepository.InsertAsync(newCartDto, ct);
    //    await _unitOfWork.SaveChangesAsync(ct);
    //}
}