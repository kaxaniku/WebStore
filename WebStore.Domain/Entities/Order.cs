namespace WebStore.Domain;
public sealed class Order
{
    public int Id { get; private set; }
    public decimal TotalPrice { get; private set; }

    private Order()
    {
    }

    public static Order Create(decimal totalPrice)
    {
        if (totalPrice < 0)
            throw new ArgumentException("Total price cannot be negative.", nameof(totalPrice));
        return new Order
        {
            TotalPrice = totalPrice,
        };
    }

    public static void SetId(Order order, int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be a positive integer.", nameof(id));
        order.Id = id;
    }

    public static void UpdateTotalPrice(Order order, decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Total price cannot be negative.", nameof(newPrice));
        order.TotalPrice = newPrice;
    }
}
