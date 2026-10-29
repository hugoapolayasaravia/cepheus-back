using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class SolicitudNotaCreditoDetalleConfiguration : IEntityTypeConfiguration<SolicitudNotaCreditoDetalle>
    {
        public void Configure(EntityTypeBuilder<SolicitudNotaCreditoDetalle> builder)
        {
            builder.ToTable("SolicitudesNotaCreditoDetalle", schema: "facturacion");

            builder.HasKey(x => new { x.Year, x.Month, x.Code, x.TipoProductoCode, x.ProductoCode, x.LineKey });

            builder.Property(x => x.Year).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Month).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.TipoProductoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ProductoCode).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.LineKey).IsRequired();

            builder.Property(x => x.Line).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.Quantity).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(x => x.UnitPrice).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(x => x.Total).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.OrderCode).HasColumnType("char(8)");

            builder.HasOne(x => x.Producto).WithMany()
                .HasForeignKey(x => new { x.TipoProductoCode, x.ProductoCode })
                .HasPrincipalKey(p => new { p.TipoProductoCode, p.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.Code, x.Year, x.Month, x.Line });
        }
    }
}
