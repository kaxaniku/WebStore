using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebStore.Domain;

namespace WebStore.Application.DTOs;

public sealed class Category
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    [Column(TypeName = "VARCHAR")]
    public string Name { get; set; } = null!;

    public ActivityInfo Activity { get; set; } = null!;
}
