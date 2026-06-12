using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebStore.CartApp.Interfaces;

namespace WebStore.CartApp.DTOs;

public sealed class Customer : IDisable
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    [Column(TypeName = "NVARCHAR")]
    public string Username { get; set; } = null!;
    public ActivityInfo Activity { get; set; } = new();
}
