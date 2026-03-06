using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Application.DTOs;

public record Product
{
    [Key]
    public int Id;

    [MaxLength(20)]
    [Column(TypeName = "VARCHAR")]
    public string Name = null!;

    [Column(TypeName = "MONEY")]
    public decimal Price;

    [MaxLength(300)]
    [Column(TypeName = "VARCHAR")]
    public string? Description;

    public int Quantity;
}
