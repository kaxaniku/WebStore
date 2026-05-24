namespace WebStore.Contracts.Cart;

public record CartItemMessage(int ProductId, string ProductName, decimal Price, int Quantity);
