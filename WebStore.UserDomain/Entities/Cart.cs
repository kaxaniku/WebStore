namespace WebStore.UserDomain.Entities;

public sealed class Cart
{
    public int Id { get; private set; }
    public Customer Customer { get; private set; } = null!;

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
}
