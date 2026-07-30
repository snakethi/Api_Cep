using ApiCep.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiCep.Infrastructure.Data.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();

            builder.Property(x => x.Email).HasMaxLength(200).IsRequired();

            builder.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();

            builder.Property(x => x.IsActive).IsRequired();

            builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime2").IsRequired();

            builder.Property(x => x.UpdatedAtUtc).HasColumnType("datetime2");

            builder.Property(x => x.DeletedAtUtc).HasColumnType("datetime2");

            builder.HasIndex(x => x.Email).IsUnique();

        }
    }
}
