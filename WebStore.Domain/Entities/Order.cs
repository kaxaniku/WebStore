namespace WebStore.Domain;

public sealed class Order
{
    private List<OrderItem> _items = new();
    public int Id { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal TotalPrice => _items.Sum(item => item.TotalPrice);

    private Order() { }

    public static Order Create(Customer customer, Cart cart)
    {
        if (customer == null) throw new ArgumentNullException(nameof(customer));

        return new Order
        {
            Customer = customer,
            CreatedAt = DateTime.UtcNow,
            _items = cart.Items.Select(OrderItem.Create).ToList()
        };
    }

    public static void SetId(Order order, int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be a positive integer.", nameof(id));
        order.Id = id;
    }
}
