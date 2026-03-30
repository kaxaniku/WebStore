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

    public void AddOrUpdateItem(Product product, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.Product.Id == product.Id);

        if (existing != null)
        {
            CartItem.AddQuantity(existing, quantity);
        }
        else
        {
            _items.Add(CartItem.Create(product, quantity));
        }
    }

    public void Clear() => _items.Clear();
}
