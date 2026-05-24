namespace WebStore.OrderDomain.Entities;

public sealed class Order
{
    private List<OrderItem> _items = new();
    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal TotalPrice => _items.Sum(item => item.TotalPrice);

    private Order() { }

    public static Order Create(int customerId, List<OrderItem> items)
    {
        if (customerId <= 0) throw new ArgumentException("Customer ID must be a positive integer.", nameof(customerId));

        return new Order
        {
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow,
            _items = items ?? new List<OrderItem>()
        };
    }

    public static void SetId(Order order, int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be a positive integer.", nameof(id));
        order.Id = id;
    }

    public sealed class OrderItem
    {
        public int Id { get; private set; }
        public int OrderId { get; private set; }
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice => UnitPrice * Quantity;

        private OrderItem() { }

        public static OrderItem Create(int orderId, int quantity, decimal unitPrice, int productId)
        {
            return new OrderItem
            {
                OrderId = orderId,
                Quantity = quantity,
                ProductId = productId,
                UnitPrice = unitPrice
            };
        }

        public static void SetId(OrderItem orderitem, int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be a positive integer.", nameof(id));
            orderitem.Id = id;
        }

        public static void UpdateUnitPrice(OrderItem orderitem, decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.");
            orderitem.UnitPrice = newPrice;
        }
    }
}
