using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebStore.CatalogApp.Interfaces;

namespace WebStore.CatalogApp.DTOs;

public sealed class Category : IDisable
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    [Column(TypeName = "VARCHAR")]
    public string Name { get; set; } = null!;

    public ActivityInfo Activity { get; set; } = new();
}
