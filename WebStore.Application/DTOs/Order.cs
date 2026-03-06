using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Application.DTOs;

public record Order
{
    [Key]
    public int Id;

    public Cart cart = null!;

    [Column(TypeName = "MONEY")]
    public decimal TotalPrice;
}
