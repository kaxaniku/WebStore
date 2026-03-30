namespace WebStore.Domain;

public sealed class CartItem
{
    public int Id { get; private set; }
    public Product Product { get; private set; } = null!;
    public int Quantity { get; private set; }
    public DateTime AddedAt { get; private set; }

    private CartItem() { }

    internal static CartItem Create(Product product, int quantity)
    {
        if(product == null) throw new ArgumentNullException(nameof(product));
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
        if (product.Quantity < product.Quantity + quantity)
            throw new InvalidOperationException("Not enough stock available.");
        Product.UpdateStock(product, product.Quantity - quantity);

        return new CartItem
        {
            Product = product,
            Quantity = quantity,
            AddedAt = DateTime.UtcNow
        };
    }

    internal static void AddQuantity(CartItem cartitem, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be at least 1.");
        if (cartitem.Product.Quantity < cartitem.Quantity + quantity)
            throw new InvalidOperationException("Not enough stock available.");
        cartitem.Quantity += quantity;
        Product.UpdateStock(cartitem.Product, cartitem.Product.Quantity - quantity);
    }

    internal static void RemoveQuantity(CartItem cartitem, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be at least 1.");
        if (cartitem.Quantity < quantity)
            throw new InvalidOperationException("Cannot remove more than the current quantity.");
        cartitem.Quantity -= quantity;
        Product.UpdateStock(cartitem.Product, cartitem.Product.Quantity + quantity);
    }
}
