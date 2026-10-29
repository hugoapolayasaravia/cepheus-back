using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class ProveedorContactoConfiguration : IEntityTypeConfiguration<ProveedorContacto>
    {
        public void Configure(EntityTypeBuilder<ProveedorContacto> builder)
        {
            builder.ToTable("ProveedorContactos", schema: "logistica");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProveedorCode)
                .IsRequired()
                 .HasColumnType("char(5)");

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .HasMaxLength(100);

            builder.Property(x => x.Position)
                .HasMaxLength(100);

            builder.Property(x => x.Phone)
                .HasMaxLength(30);

            builder.Property(x => x.MobilePhone)
                .HasMaxLength(30);

            builder.Property(x => x.Email)
                .HasMaxLength(150);

            builder.Property(x => x.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.HasOne(x => x.Proveedor)
                .WithMany()
                .HasForeignKey(x => x.ProveedorCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ProveedorCode);
        }
    }
}
