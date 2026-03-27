using WebStore.Domain.ValueObjects;

namespace WebStore.Domain;
public sealed class Customer
{
    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; } = null!;

    private Customer()
    {
    }

    public static Customer Create(string username, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        return new Customer
        {
            Username = username,
            Email = EmailAddress.Create(email),
            PasswordHash = Password.Create(passwordHash)
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

    public static void UpdateEmail(Customer customer, string newEmail)
    {
        string email = EmailAddress.Create(newEmail);
        customer.Email = newEmail;
    }

    public static void SetNewPassword(Customer customer, string oldPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(oldPassword))
            throw new ArgumentException("Password cannot be null or empty.", nameof(oldPassword));
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("New password cannot be null or empty.", nameof(newPassword));
        if(newPassword == oldPassword)
            throw new ArgumentException("New password cannot be the same as the old password.", nameof(newPassword));
        if (customer.PasswordHash != Password.Create(oldPassword))
            throw new ArgumentException("Old password is incorrect.", nameof(oldPassword));
        customer.PasswordHash = Password.Create(newPassword);
    }
}
