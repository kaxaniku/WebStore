using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebStore.Domain;

namespace WebStore.Application.DTOs;

public sealed class Product
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    [Column(TypeName = "VARCHAR")]
    public string Name { get; set; } = null!;

    [Column(TypeName = "MONEY")]
    public decimal Price { get; set; }

    [MaxLength(300)]
    [Column(TypeName = "VARCHAR")]
    public string? Description { get; set; }

    public int Quantity { get; set; }

    public Category Category { get; set; } = null!;

    public ActivityInfo Activity { get; set; } = null!;
}
