using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class CotizacionDetalleConfiguration : IEntityTypeConfiguration<CotizacionDetalle>
    {
        public void Configure(EntityTypeBuilder<CotizacionDetalle> builder)
        {
            builder.ToTable("CotizacionesDetalle", schema: "facturacion");

            builder.HasKey(x => new { x.NegocioCode, x.Year, x.Month, x.Code, x.Item });

            builder.Property(x => x.NegocioCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Year).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Month).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(8)");

            builder.Property(x => x.Item)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ProductoTipoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ProductoCode).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.UnitCode).IsRequired().HasMaxLength(5).HasDefaultValue("");

            builder.Property(x => x.Quantity).IsRequired().HasColumnType("decimal(18,6)").HasDefaultValue(0m);
            builder.Property(x => x.UnitPrice).IsRequired().HasColumnType("decimal(18,4)").HasDefaultValue(0m);

            builder.Ignore(x => x.Total);

            builder.Property(x => x.Observations).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Order).IsRequired();
            builder.Property(x => x.DeliveredQuantity).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Cotizacion).WithMany(c => c.Detalles)
                .HasForeignKey(x => new { x.NegocioCode, x.Year, x.Month, x.Code })
                .HasPrincipalKey(c => new { c.NegocioCode, c.Year, c.Month, c.Code })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Producto).WithMany()
                .HasForeignKey(x => new { x.ProductoTipoCode, x.ProductoCode })
                .HasPrincipalKey(p => new { p.TipoProductoCode, p.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Unit).WithMany()
                .HasForeignKey(x => x.UnitCode)
                .HasPrincipalKey(u => u.Code)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
