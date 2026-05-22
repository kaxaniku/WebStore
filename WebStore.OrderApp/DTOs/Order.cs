using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.OrderApp.DTOs;

public sealed class Order
{
    [Key]
    public int Id { get; set; }

    public int CustomerId { get; set; }

    [Column(TypeName = "MONEY")]
    public decimal TotalPrice { get; set; }

    public DateTime OrderProcessedDate { get; set; } = DateTime.UtcNow;

    public List<OrderItem>? Items { get; set; } = new();
}
