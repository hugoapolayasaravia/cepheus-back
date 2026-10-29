using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class ComprobanteVentaDetalleConfiguration : IEntityTypeConfiguration<ComprobanteVentaDetalle>
    {
        public void Configure(EntityTypeBuilder<ComprobanteVentaDetalle> builder)
        {
            builder.ToTable("ComprobantesVentaDetalle", schema: "facturacion");

            builder.HasKey(x => new { x.TypeCode, x.Number, x.Line });

            builder.Property(x => x.TypeCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Number).IsRequired().HasColumnType("char(10)");
            builder.Property(x => x.GuiaNumber).HasColumnType("char(10)");
            builder.Property(x => x.OrdenTrabajoCode).HasColumnType("char(8)");
            builder.Property(x => x.ProductoTipoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ProductoCode).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Quantity).IsRequired().HasColumnType("decimal(18,6)");
            builder.Property(x => x.UnitPrice).IsRequired().HasColumnType("decimal(18,8)");
            builder.Property(x => x.Total).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.GuiaDetalle).WithMany()
                .HasForeignKey(x => new { x.GuiaNumber, x.GuiaLine })
                .HasPrincipalKey(g => new { g.GuiaNumber, g.Line })
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Producto).WithMany()
                .HasForeignKey(x => new { x.ProductoTipoCode, x.ProductoCode })
                .HasPrincipalKey(p => new { p.TipoProductoCode, p.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.GuiaNumber, x.GuiaLine });
            builder.HasIndex(x => new { x.ProductoTipoCode, x.ProductoCode });
        }
    }
}
