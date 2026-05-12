using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebStore.UserApp.DTOs;

namespace WebStore.UserInfrastructure.Repositories.EntityConfigurations;

internal class AdminConfig : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder
            .HasIndex(a => a.Username)
            .IsUnique();

        builder
        .ToTable("Admins")
        .ComplexProperty(a => a.Activity);
    }
}
