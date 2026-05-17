namespace WebStore.CartDomain.Entities;

public sealed class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    private Product() { }

    public static Product Create(string name,decimal price, int quantity)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));
        if (quantity < 0)
            throw new ArgumentException("Stock cannot be negative.", nameof(quantity));
        return new Product
        {
            Name = name,
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

    public static void UpdateName(Product product, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(newName));
        product.Name = newName;
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
