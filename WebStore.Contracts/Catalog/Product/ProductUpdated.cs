namespace WebStore.Contracts.Catalog.Product;

public record ProductUpdated
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
}
