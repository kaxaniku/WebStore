namespace WebStore.Contracts.Cart;

public record GetCartResponse(int CustomerId, List<CartItemMessage> Items);
