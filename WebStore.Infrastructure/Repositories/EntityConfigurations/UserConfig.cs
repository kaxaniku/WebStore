using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebStore.Application.DTOs;

namespace WebStore.Infrastructure.Repositories.EntityConfigurations;

internal class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasIndex(a => a.Username)
            .IsUnique();
            
        builder.ToTable("Users");
    }
}
