using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Application.DTOs;

public sealed class Order
{
    [Key]
    public int Id { get; set; }

    public Cart cart { get; set; } = null!;

    [Column(TypeName = "MONEY")]
    public decimal TotalPrice { get; set; }
}
