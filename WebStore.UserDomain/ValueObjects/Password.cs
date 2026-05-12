using System.Security.Cryptography;
using System.Text;

public record Password
{
    public string Hash { get; }

    private Password(string hash)
    {
        Hash = hash;
    }

    public static Password Create(string plaintext)
    {
        if (string.IsNullOrWhiteSpace(plaintext) || plaintext.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters long.");

        if (!plaintext.Any(char.IsUpper) || !plaintext.Any(char.IsDigit))
            throw new ArgumentException("Password must contain an uppercase letter and a number.");

        string hashedValue = HashPassword(plaintext);

        return new Password(hashedValue);
    }

    public static implicit operator string(Password password) => password.Hash;

    private static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToBase64String(hashBytes);
    }
}