using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class NotaDebitoVentaDetalleConfiguration : IEntityTypeConfiguration<NotaDebitoVentaDetalle>
    {
        public void Configure(EntityTypeBuilder<NotaDebitoVentaDetalle> builder)
        {
            builder.ToTable("NotasDebitoVentaDetalle", schema: "facturacion");

            builder.HasKey(x => new { x.PlantaCode, x.Number, x.Line });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Number).IsRequired().HasMaxLength(10);
            builder.Property(x => x.Line).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.TipoProductoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ProductoCode).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Description).IsRequired().HasMaxLength(255);

            builder.Property(x => x.Quantity).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.UnitPrice).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(x => x.Total).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(x => x.LineKey).IsRequired().HasDefaultValue(0);

            builder.HasOne(x => x.Producto).WithMany()
                .HasForeignKey(x => new { x.TipoProductoCode, x.ProductoCode })
                .HasPrincipalKey(p => new { p.TipoProductoCode, p.Code })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
