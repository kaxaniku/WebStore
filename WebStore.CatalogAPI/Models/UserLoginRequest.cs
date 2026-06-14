namespace WebStore.CatalogAPI.Models;

public class UserLoginRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
