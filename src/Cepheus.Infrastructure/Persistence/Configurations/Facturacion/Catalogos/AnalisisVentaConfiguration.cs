using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Catalogos
{
    public class AnalisisVentaConfiguration : IEntityTypeConfiguration<AnalisisVenta>
    {
        public void Configure(EntityTypeBuilder<AnalisisVenta> builder)
        {
            builder.ToTable("AnalisisVentas", schema: "facturacion");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(3)");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(40);

            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.ShortName)
                .HasMaxLength(8);

            builder.Property(x => x.SegmentoVentasCode)
                .HasMaxLength(2);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.SegmentoVentas)
                .WithMany()
                .HasForeignKey(x => x.SegmentoVentasCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.SegmentoVentasCode);
        }
    }
}
