using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Application.DTOs;

public sealed class Cart
{
    [Key]
    [ForeignKey(nameof(Customer))]
    public int Id { get; set; }

    public Customer Customer { get; set; } = null!;
}
