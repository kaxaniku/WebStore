using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebStore.UserApp.DTOs;

namespace WebStore.UserInfrastructure.Repositories.EntityConfigurations;

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
