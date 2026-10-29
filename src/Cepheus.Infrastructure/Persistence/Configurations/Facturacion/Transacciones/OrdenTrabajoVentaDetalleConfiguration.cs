using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Cepheus.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class OrdenTrabajoVentaDetalleConfiguration : IEntityTypeConfiguration<OrdenTrabajoVentaDetalle>
    {
        public void Configure(EntityTypeBuilder<OrdenTrabajoVentaDetalle> builder)
        {
            builder.ToTable("OrdenesTrabajoVentaDetalle", schema: "facturacion");

            builder.HasKey(x => new { x.OrdenTrabajoCode, x.Item });

            builder.Property(x => x.OrdenTrabajoCode).IsRequired().HasColumnType("char(8)");

            // Clave IDENTITY global del legacy: otras tablas la usan como referencia a la línea.
            builder.Property(x => x.Item).ValueGeneratedOnAdd();

            builder.Property(x => x.ProductoTipoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ProductoCode).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.NegocioCode).IsRequired().HasColumnType("char(2)");

            builder.Property(x => x.Quantity).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.ApprovedQuantity).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.ProgrammedQuantity).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.DeliveredQuantity).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.Property(x => x.UnitPrice).IsRequired().HasColumnType("decimal(18,8)");
            builder.Property(x => x.Total).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(x => x.Status)
                .IsRequired()
                .HasColumnType("char(1)")
                .HasConversion(new LegacyCharConverter<EstadoDetalleOrdenTrabajoVenta>(
                    (EstadoDetalleOrdenTrabajoVenta.Pendiente, "P"),
                    (EstadoDetalleOrdenTrabajoVenta.Anulado, "A")));

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Producto).WithMany()
                .HasForeignKey(x => new { x.ProductoTipoCode, x.ProductoCode })
                .HasPrincipalKey(p => new { p.TipoProductoCode, p.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Negocio).WithMany()
                .HasForeignKey(x => x.NegocioCode)
                .HasPrincipalKey(n => n.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.Item).IsUnique();
            builder.HasIndex(x => new { x.ProductoTipoCode, x.ProductoCode });
        }
    }
}
