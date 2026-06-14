using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebStore.CatalogApp.Interfaces;

namespace WebStore.CatalogApp.DTOs;

public sealed class Admin : IDisable
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    [Column(TypeName = "NVARCHAR")]
    public string Username { get; set; } = null!;

    [MaxLength(100)]
    public string PasswordHash { get; set; } = null!;

    public ActivityInfo Activity { get; set; } = new();
}
