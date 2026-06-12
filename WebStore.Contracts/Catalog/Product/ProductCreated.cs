namespace WebStore.Contracts.Catalog.Product;

public record ProductCreated
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public int CategoryId { get; init; }
    public string ImagePath { get; init; } = string.Empty;
}
