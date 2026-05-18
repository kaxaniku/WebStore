using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.CartApp.DTOs;

public sealed class Cart
{
    [Key]
    [ForeignKey(nameof(Customer))]
    public int Id { get; set; }

    public Customer Customer { get; set; } = null!;

    public List<CartItem>? Items { get; set; } = new();
}
