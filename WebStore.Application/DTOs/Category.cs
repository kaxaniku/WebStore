using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Application.DTOs;

public record Category
{
    [Key]
    public int Id;

    [MaxLength(20)]
    [Column(TypeName = "VARCHAR")]
    public string Name = null!;
}
