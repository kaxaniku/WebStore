namespace WebStore.CartDomain.Entities;

public sealed class Customer
{
    public int Id { get; private set; }
    public string Username { get; private set; } = null!;
    private Customer() { }

    public static Customer Create(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        return new Customer
        {
            Username = username
        };
    }

    public static void SetId(Customer customer, int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be a positive integer.", nameof(id));
        customer.Id = id;
    }

    public static void UpdateUsername(Customer customer, string newUsername)
    {
        if (string.IsNullOrWhiteSpace(newUsername))
            throw new ArgumentException("Username cannot be null or empty.", nameof(newUsername));
        customer.Username = newUsername;
    }
}
