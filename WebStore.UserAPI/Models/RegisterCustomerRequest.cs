namespace WebStore.UserAPI.Models;

// Request DTOs
public record RegisterCustomerRequest(string Username, string Email, string Password);
