using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Catalogos
{
    public class AlturaLosaConfiguration : IEntityTypeConfiguration<AlturaLosa>
    {
        public void Configure(EntityTypeBuilder<AlturaLosa> builder)
        {
            builder.ToTable("AlturasLosa", schema: "facturacion");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.Value).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.Width).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.Property(x => x.ProductoTipoCode).IsRequired().HasMaxLength(2);
            builder.Property(x => x.ProductoCode).IsRequired().HasMaxLength(4);

            builder.Property(x => x.PolystyreneProductoTipoCode).HasMaxLength(2);
            builder.Property(x => x.PolystyreneProductoCode).HasMaxLength(4);

            builder.Property(x => x.PolystyreneValue).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.PolystyreneWidth).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Productos)
                .WithMany()
                .HasForeignKey(x => new { x.ProductoTipoCode, x.ProductoCode })
                .HasPrincipalKey(p => new { p.TipoProductoCode, p.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PolystyreneProducto)
                .WithMany()
                .HasForeignKey(x => new { x.PolystyreneProductoTipoCode, x.PolystyreneProductoCode })
                .HasPrincipalKey(p => new { p.TipoProductoCode, p.Code })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
