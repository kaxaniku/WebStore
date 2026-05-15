namespace WebStore.CartDomain.Entities;

public sealed class Product
{
    public int Id { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    private Product() { }

    public static Product Create(decimal price, int quantity)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));
        if (quantity < 0)
            throw new ArgumentException("Stock cannot be negative.", nameof(quantity));
        return new Product
        {
            Price = price,
            Stock = quantity
        };
    }

    public static void SetId(Product product, int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be a positive integer.", nameof(id));
        product.Id = id;
    }

    public static void UpdatePrice(Product product, decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(newPrice));
        product.Price = newPrice;
    }

    public static Product UpdateStock(Product product, int newStock)
    {
        if (newStock < 0)
            throw new ArgumentException("Stock cannot be negative.", nameof(newStock));
        product.Stock = newStock;
        return product;
    }
}
