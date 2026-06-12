using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebStore.CartApp.Interfaces;

namespace WebStore.CartApp.DTOs;

public sealed class Product : IDisable
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    [Column(TypeName = "NVARCHAR")]
    public string Name { get; set; } = null!;

    [Column(TypeName = "MONEY")]
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public ActivityInfo Activity { get; set; } = new();
}

