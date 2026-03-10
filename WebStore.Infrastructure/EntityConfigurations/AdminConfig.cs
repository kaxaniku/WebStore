using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebStore.Application.DTOs;

namespace Webstore.Infrastructure.EntityConfigurations;

internal class AdminConfig : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder
            .HasIndex(a => a.Username)
            .IsUnique();
            
        builder.ToTable("Admins");
    }
}
