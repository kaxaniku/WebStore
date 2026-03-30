namespace WebStore.Domain;

public sealed class OrderItem
{
    public int Id { get; private set; }
    public CartItem CartItem { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    private OrderItem() { }

    internal static OrderItem Create(CartItem cartItem)
    {
        if (cartItem == null)
            throw new ArgumentNullException(nameof(cartItem));
        return new OrderItem
        {
            CartItem = cartItem,
            Quantity = cartItem.Quantity,
            UnitPrice = cartItem.Product.Price
        };
    }

    internal static void SetId(OrderItem orderitem, int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be a positive integer.", nameof(id));
        orderitem.Id = id;
    }

    internal static void UpdateUnitPrice(OrderItem orderitem, decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.");
        orderitem.UnitPrice = newPrice;
    }
}
