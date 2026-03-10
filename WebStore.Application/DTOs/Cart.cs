using System.ComponentModel.DataAnnotations;

namespace WebStore.Application.DTOs;

public sealed class Cart
{
    [Key]
    public int Id { get; set; }

    public Customer Customer { get; set; } = null!;
}
