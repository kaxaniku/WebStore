namespace WebStore.CatalogAPI.Models;

public record ProductModel(string Name, decimal Price, string? Description, int Stock, int CategoryId, IFormFile? ImageFile);