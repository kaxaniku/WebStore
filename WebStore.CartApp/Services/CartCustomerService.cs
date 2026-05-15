using MapsterMapper;
using WebStore.CartApp.Interfaces.Repositories;
using WebStore.CartApp.Interfaces.Services;
using WebStore.CartDomain.Entities;

namespace WebStore.CartApp.Services;

public class CartCustomerService : ICartCustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartCustomerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> RegisterCartCustomerAsync(int id, string username, CancellationToken cancellationToken)
    {
        var customerEntity = Customer.Create(username);
        Customer.SetId(customerEntity, id);
        var dto = _mapper.Map<DTOs.Customer>(customerEntity);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            await _unitOfWork.CustomerRepository.InsertAsync(dto, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return customerEntity.Id;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw new Exception("An error occurred while registering the customer.", ex);
        }
    }

    public async Task UpdateUsernameAsync(int customerId, string newUsername, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer == null) throw new KeyNotFoundException();
        var customerEntity = _mapper.Map<Customer>(customer);
        Customer.UpdateUsername(customerEntity, newUsername);
        _mapper.Map(customerEntity, customer);

        await _unitOfWork.CustomerRepository.UpdateAsync(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveCartCustomerAsync(int customerId, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, cancellationToken)
                ?? throw new KeyNotFoundException("Customer not found");

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _unitOfWork.CustomerRepository.Delete(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw new Exception("An error occurred while removing the customer.", ex);
        }
    }

}