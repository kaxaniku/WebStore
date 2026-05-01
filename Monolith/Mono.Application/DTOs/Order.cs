using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Application.DTOs;

public sealed class Order
{
    [Key]
    public int Id { get; set; }

    public Customer Customer { get; set; } = null!;

    [Column(TypeName = "MONEY")]
    public decimal TotalPrice { get; set; }

    public DateTime OrderProcessedDate { get; set; } = DateTime.UtcNow;
}
