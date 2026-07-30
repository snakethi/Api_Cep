using ApiCep.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiCep.Infrastructure.Configurations
{
    public sealed class AddressConfiguration
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.ZipCode).HasMaxLength(8).IsRequired();

            builder.Property(x => x.Street).HasMaxLength(200).IsRequired();

            builder.Property(x => x.Number).HasMaxLength(20).IsRequired();

            builder.Property(x => x.Neighborhood).HasMaxLength(100).IsRequired();

            builder.Property(x => x.City).HasMaxLength(100).IsRequired();

            builder.Property(x => x.State).HasMaxLength(2).IsRequired();

            builder.Property(x => x.Complement).HasMaxLength(200);

            builder.Property(x => x.IsActive).IsRequired();

            builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime2").IsRequired();

            builder.Property(x => x.UpdatedAtUtc).HasColumnType("datetime2");

            builder.Property(x => x.DeletedAtUtc).HasColumnType("datetime2");

            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict).IsRequired();

            builder.HasIndex(x => x.UserId);


        }
    }
}
