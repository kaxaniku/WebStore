using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Application.DTOs;

public enum Role : byte
{
    Owner = 0,
    Admin = 1,
    User = 2
}

public record User
{
    [Key]
    public int Id;

    [MaxLength(20)]
    [Column(TypeName = "VARCHAR")]
    public string Username = null!;

    [MaxLength(50)]
    [Column(TypeName = "VARCHAR")]
    public string Email = null!;

    [MaxLength(100)]
    public string PasswordHash = null!;

    public Role Role = Role.User;

    public ICollection<Order>? Orders { get; set; }
}
