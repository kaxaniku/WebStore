using WebStore.OrderDomain.Entities;

namespace WebStore.OrderApp.Interfaces.Services;

public interface IOrderProductService
{
    Task<int> CreateOrderProductAsync(int id, string name, decimal price, int quantity, CancellationToken ct);
    Task DeleteOrderProductAsync(int id, CancellationToken ct);
    Task UpdateProductNameAsync(int id, string newName, CancellationToken ct);
    Task UpdateProductPriceAsync(int id, decimal newPrice, CancellationToken ct);
    Task UpdateProductStockAsync(int id, int newStock, CancellationToken ct);
    Task UpdateMainProductStockAsync(DTOs.Product productDto, Product productEntity, CancellationToken ct);
}