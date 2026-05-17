using System.ComponentModel.DataAnnotations;

namespace WebStore.CartApp.DTOs;

public sealed class CartItem
{
    [Key]
    public int Id { get; set; }
    public int CartId { get; set; }
    public Cart? Cart { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public DateTime ItemAddedDate { get; set; } = DateTime.UtcNow;

    public int Quantity { get; set; }
}
