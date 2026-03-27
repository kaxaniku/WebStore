using System.Text.RegularExpressions;

namespace WebStore.Domain.ValueObjects;

public record EmailAddress
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.");

        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException($"Invalid email format: {value}");

        Value = value.ToLowerInvariant().Trim();
    }

    public static EmailAddress Create(string value) => new EmailAddress(value);

    public static implicit operator string(EmailAddress email) => email.Value;

    public override string ToString() => Value;
}