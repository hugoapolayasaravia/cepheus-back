using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Catalogos
{
    public class ListaPrecioConfiguration : IEntityTypeConfiguration<ListaPrecio>
    {
        public void Configure(EntityTypeBuilder<ListaPrecio> builder)
        {
            builder.ToTable("ListasPrecios", schema: "facturacion");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.TipoProductoCode)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.ProductoCode)
                .IsRequired()
                .HasColumnType("char(4)");

            builder.Property(x => x.Precio)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(x => x.FechaInicio)
                .IsRequired();

            builder.Property(x => x.FechaFin);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.Producto)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.TipoProductoCode,
                    x.ProductoCode
                })
                .HasPrincipalKey(x => new
                {
                    x.TipoProductoCode,
                    x.Code
                })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}