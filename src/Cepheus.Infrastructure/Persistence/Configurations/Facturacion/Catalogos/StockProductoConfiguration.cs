using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Catalogos
{
    public class StockProductoConfiguration : IEntityTypeConfiguration<StockProducto>
    {
        public void Configure(EntityTypeBuilder<StockProducto> builder)
        {
            builder.ToTable("StockProductos", schema: "facturacion");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.TipoProductoCode)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.ProductoCode)
                .IsRequired()
                .HasColumnType("char(4)");

            builder.Property(x => x.Cantidad).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasIndex(x => new { x.PlantaCode, x.TipoProductoCode, x.ProductoCode }).IsUnique();

            builder.HasOne(x => x.Planta)
                .WithMany()
                .HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Producto)
                .WithMany()
                .HasForeignKey(x => new { x.TipoProductoCode, x.ProductoCode })
                .HasPrincipalKey(p => new { p.TipoProductoCode, p.Code })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
