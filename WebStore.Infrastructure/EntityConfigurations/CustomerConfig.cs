using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebStore.Application.DTOs;

namespace Webstore.Infrastructure.EntityConfigurations;

internal class CustomerConfig : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder
            .HasIndex(a => a.Username)
            .IsUnique();
            
        builder.ToTable("Customers");
    }
}
