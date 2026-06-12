namespace WebStore.CatalogDomain.Entities;

public sealed class Product
{
    public int Id { get; private set; }
    public int CategoryId { get; private set; } = 0;
    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }
    public string? Description { get; private set; }
    public int Stock { get; private set; }
    public string ImagePath { get; set; } = null!;

    private Product() { }

    public static Product Create(string name, decimal price, string? description, int quantity, int categoryId, string imagePath = "")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name name cannot be null or empty.", nameof(name));
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));
        if (quantity < 0)
            throw new ArgumentException("Stock cannot be negative.", nameof(quantity));
        if (categoryId < 0)
            throw new ArgumentException("CategoryId cannot be negative.", nameof(categoryId));
        return new Product
        {
            Name = name,
            Description = description,
            Price = price,
            Stock = quantity,
            CategoryId = categoryId,
            ImagePath = imagePath
        };
    }

    public static void SetId(Product product, int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be a positive integer.", nameof(id));
        product.Id = id;
    }

    public static void UpdateCategory(Product product, int newCategoryId)
    {
        if (newCategoryId < 0)
            throw new ArgumentException("CategoryId cannot be negative.", nameof(newCategoryId));
        product.CategoryId = newCategoryId;
    }

    public static void UpdateDesc(Product product, string newName, string? description)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be null or empty.", nameof(newName));
        product.Name = newName;
        product.Description = description;
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

    public static void UpdateImagePath(Product product, string newImagePath)
    {
        if (string.IsNullOrWhiteSpace(newImagePath))
            throw new ArgumentException("ImagePath cannot be null or empty.", nameof(newImagePath));
        product.ImagePath = newImagePath;
    }
}
