using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Catalogos
{
    public class CategoriaProductoConfiguration : IEntityTypeConfiguration<CategoriaProducto>
    {
        public void Configure(EntityTypeBuilder<CategoriaProducto> builder)
        {
            builder.ToTable("CategoriasProducto", schema: "facturacion");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(3)");

            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);

            builder.Property(x => x.FirthCode).HasMaxLength(2);
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

        }
    }
}
