using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class GuiaRemisionVentaDetalleConfiguration : IEntityTypeConfiguration<GuiaRemisionVentaDetalle>
    {
        public void Configure(EntityTypeBuilder<GuiaRemisionVentaDetalle> builder)
        {
            builder.ToTable("GuiasRemisionVentaDetalle", schema: "facturacion");

            builder.HasKey(x => new { x.GuiaNumber, x.Line });

            builder.Property(x => x.GuiaNumber).IsRequired().HasColumnType("char(10)");
            builder.Property(x => x.OrdenTrabajoCode).HasColumnType("char(8)");
            builder.Property(x => x.ProductoTipoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ProductoCode).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Quantity).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.TopeQuantity).HasColumnType("decimal(18,2)");
            builder.Property(x => x.UnitPrice).IsRequired().HasColumnType("decimal(18,4)");

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.OrdenTrabajoDetalle).WithMany()
                .HasForeignKey(x => new { x.OrdenTrabajoCode, x.OrdenTrabajoItem })
                .HasPrincipalKey(d => new { d.OrdenTrabajoCode, d.Item })
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Producto).WithMany()
                .HasForeignKey(x => new { x.ProductoTipoCode, x.ProductoCode })
                .HasPrincipalKey(p => new { p.TipoProductoCode, p.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.OrdenTrabajoCode, x.OrdenTrabajoItem });
            builder.HasIndex(x => new { x.ProductoTipoCode, x.ProductoCode });
        }
    }
}
