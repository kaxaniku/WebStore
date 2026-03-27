using WebStore.Domain.ValueObjects;

namespace WebStore.Domain;
public sealed class Admin
{
    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; } = null!;

    private Admin()
    {
    }

    public static Admin Create(string username, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        return new Admin
        {
            Username = username,
            Email = EmailAddress.Create(email),
            PasswordHash = Password.Create(passwordHash)
        };
    }

    public static void SetId(Admin admin, int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be a positive integer.", nameof(id));
        admin.Id = id;
    }

    public static void UpdateUsername(Admin admin, string newUsername)
    {
        if (string.IsNullOrWhiteSpace(newUsername))
            throw new ArgumentException("Username cannot be null or empty.", nameof(newUsername));
        admin.Username = newUsername;
    }

    public static void UpdateEmail(Admin admin, string newEmail)
    {
        string email = EmailAddress.Create(newEmail);
        admin.Email = newEmail;
    }

    public static void SetNewPassword(Admin admin, string oldPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(oldPassword))
            throw new ArgumentException("Password cannot be null or empty.", nameof(oldPassword));
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("New password cannot be null or empty.", nameof(newPassword));
        if(newPassword == oldPassword)
            throw new ArgumentException("New password cannot be the same as the old password.", nameof(newPassword));
        if (admin.PasswordHash != Password.Create(oldPassword))
            throw new ArgumentException("Old password is incorrect.", nameof(oldPassword));
        admin.PasswordHash = Password.Create(newPassword);
    }
}
