using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebStore.UserApp.DTOs;

namespace WebStore.UserInfrastructure.Repositories.EntityConfigurations;

internal class CustomerConfig : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder
            .HasIndex(a => a.Username)
            .IsUnique();

        builder.ToTable("Customers")
        .ComplexProperty(c => c.Activity);
    }
}
