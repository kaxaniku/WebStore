namespace WebStore.Domain;

public sealed class Cart
{
    public int Id { get; private set; }
    public Customer Customer { get; private set; } = null!;
    private readonly List<CartItem> _items = new();
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(item => item.Product.Price * item.Quantity);

    private Cart() { }

    public static Cart Create(Customer customer)
    {
        if (customer == null) throw new ArgumentNullException(nameof(customer));

        return new Cart
        {
            Id = customer.Id,
            Customer = customer
        };
    }

    public static void AddOrUpdateItem(Cart cart, Product product, int quantity)
    {
        var items = cart._items;
        var existing = items.FirstOrDefault(i => i.Product.Id == product.Id);

        if (existing != null)
        {
            CartItem.AddQuantity(existing, quantity);
        }
        else
        {
            items.Add(CartItem.Create(product, quantity));
        }
    }

    public static void Clear(Cart cart) => cart._items.Clear();
}
