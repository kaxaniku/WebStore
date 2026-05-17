namespace WebStore.CartDomain.Entities;

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

    public static CartItem AddOrUpdateItem(Cart cart, Product product, int quantity)
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

        return existing ?? items.Last();
    }

    public static Cart Clear(Cart cart) 
    {
        cart._items.Clear();
        return cart;
    } 

    public sealed class CartItem
    {
        public int Id { get; private set; }
        public Product Product { get; private set; } = null!;
        public int Quantity { get; private set; }
        public DateTime AddedAt { get; private set; }

        private CartItem() { }

        public static CartItem Create(Product product, int quantity)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
            if (product.Stock - quantity < 0)
                throw new InvalidOperationException("Not enough stock available.");

            return new CartItem
            {
                Product = product,
                Quantity = quantity,
                AddedAt = DateTime.UtcNow
            };
        }

        public static void AddQuantity(CartItem cartitem, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be at least 1.");
            if (cartitem.Product.Stock - (cartitem.Quantity + quantity) < 0)
                throw new InvalidOperationException("Not enough stock available.");
            cartitem.Quantity += quantity;
        }

        public static CartItem RemoveQuantity(CartItem cartitem, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be at least 1.");
            if (cartitem.Quantity < quantity)
                throw new InvalidOperationException("Cannot remove more than the current quantity.");
            cartitem.Quantity -= quantity;
            return cartitem;
        }
    }
}
