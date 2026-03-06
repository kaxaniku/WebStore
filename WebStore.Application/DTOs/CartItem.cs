using System.ComponentModel.DataAnnotations;

namespace WebStore.Application.DTOs;

public sealed class CartItem
{
    [Key]
    public int Id { get; set; }

    public Cart Cart { get; set; } = null!;

    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
}
