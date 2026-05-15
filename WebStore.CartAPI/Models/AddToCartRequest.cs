namespace WebStore.CartAPI.Models;

public record AddToCartRequest(int ProductId, int Quantity);