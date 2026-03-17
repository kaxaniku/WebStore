namespace WebStore.Domain.Entities;

public class CustomerEntity
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    protected CustomerEntity(int id, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.");
        if(id <= 0)
            throw new ArgumentException("Id must be a positive integer.");

        Id = id;
        Email = email.ToLower().Trim();
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }

    public void ChangeEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail)) throw new Exception("Invalid email.");

        Email = newEmail.ToLower().Trim();
        IsEmailConfirmed = false;
    }

    public void ConfirmEmail()
    {
        IsEmailConfirmed = true;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash)) throw new Exception("Invalid password hash.");
        PasswordHash = newPasswordHash;
    }
}
