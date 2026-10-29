using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Catalogos
{
    public class PrecioProductoConfiguration : IEntityTypeConfiguration<PrecioProducto>
    {
        public void Configure(EntityTypeBuilder<PrecioProducto> builder)
        {
            builder.ToTable("PreciosProducto", schema: "facturacion");

            builder.HasKey(x => new
            {
                x.FleteCode,
                x.ProductoTipoCode,
                x.ProductoCode,
                x.CurrencyTypeCode,
                x.CurrencyCode
            });

            builder.Property(x => x.FleteCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ProductoTipoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ProductoCode).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.CurrencyTypeCode).IsRequired().HasColumnType("char(1)");
            builder.Property(x => x.CurrencyCode).IsRequired().HasColumnType("char(1)").HasDefaultValue("S");

            builder.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18,4)").HasDefaultValue(0m);
            builder.Property(x => x.TransportAmount).IsRequired().HasColumnType("decimal(18,4)").HasDefaultValue(0m);
            builder.Property(x => x.FreightAmount).IsRequired().HasColumnType("decimal(18,4)").HasDefaultValue(0m);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Flete)
                .WithMany()
                .HasForeignKey(x => x.FleteCode)
                .HasPrincipalKey(f => f.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Producto)
                .WithMany()
                .HasForeignKey(x => new { x.ProductoTipoCode, x.ProductoCode })
                .HasPrincipalKey(p => new { p.TipoProductoCode, p.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.ProductoTipoCode, x.ProductoCode });
        }
    }
}
