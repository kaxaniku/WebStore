using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Application.DTOs;

public sealed class OrderItem
{
    [Key]
    public int Id { get; set; }

    public Order Order { get; set; } = null!;

    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    [Column(TypeName = "MONEY")]
    public decimal ItemPrice { get; set; }

    public DateTime ItemAddedDate { get; set; } = DateTime.UtcNow;
}
