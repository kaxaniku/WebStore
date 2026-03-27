using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebStore.Domain.ValueObjects;

namespace WebStore.Domain;
public sealed class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string? Description { get; private set; }
    public int Quantity { get; private set; }

    private Product()
    {
    }

    public static Product Create(string name, decimal price, string? description, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name name cannot be null or empty.", nameof(name));
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
        return new Product
        {
            Name = name,
            Description = description,
            Price = price,
            Quantity = quantity
        };
    }

    public static void SetId(Product product, int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be a positive integer.", nameof(id));
        product.Id = id;
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

    public static void UpdateQuantity(Product product, int newQuantity)
    {
        if (newQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(newQuantity));
        product.Quantity = newQuantity;
    }
}
