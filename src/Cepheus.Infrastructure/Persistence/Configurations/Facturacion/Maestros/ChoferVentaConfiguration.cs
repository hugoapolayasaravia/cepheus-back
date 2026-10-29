using Cepheus.Domain.Facturacion.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Maestros
{
    public class ChoferVentaConfiguration : IEntityTypeConfiguration<ChoferVenta>
    {
        public void Configure(EntityTypeBuilder<ChoferVenta> builder)
        {
            builder.ToTable("ChoferesVentas", schema: "facturacion");

            builder.HasKey(x => new { x.TransportistaCode, x.Code });

            builder.Property(x => x.TransportistaCode)
                .IsRequired()
                 .HasColumnType("char(4)");

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(4)");

            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.DriverLicenseNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Observations)
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.TransportistaVenta)
                .WithMany()
                .HasForeignKey(x => x.TransportistaCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
