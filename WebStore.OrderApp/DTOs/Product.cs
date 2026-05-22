using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebStore.OrderApp.Interfaces;

namespace WebStore.OrderApp.DTOs;

public sealed class Product : IDisable
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    [Column(TypeName = "VARCHAR")]
    public string Name { get; set; } = null!;

    [Column(TypeName = "MONEY")]
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public ActivityInfo Activity { get; set; } = new();
}

