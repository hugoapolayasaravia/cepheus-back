using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class HojaCorteDetalleConfiguration
        : IEntityTypeConfiguration<HojaCorteDetalle>
    {
        public void Configure(EntityTypeBuilder<HojaCorteDetalle> builder)
        {
            builder.ToTable("HojaCorteDetalles", schema: "facturacion");

            // Clave primaria compuesta de la línea.
            builder.HasKey(x => new
            {
                x.Year,
                x.HojaCorteCode,
                x.ProductoTipoCode,
                x.ProductoCode,
                x.ItemNumber
            });

            builder.Property(x => x.Year)
                .IsRequired()
                .HasColumnType("char(4)");

            builder.Property(x => x.HojaCorteCode)
                .IsRequired()
                .HasColumnType("char(8)");

            builder.Property(x => x.ProductoTipoCode)
                .IsRequired()
                .HasMaxLength(2);

            builder.Property(x => x.ProductoCode)
                .IsRequired()
                .HasMaxLength(8);

            builder.Property(x => x.ItemNumber)
                .IsRequired();

            builder.Property(x => x.UnitCode)
                .IsRequired()
                .HasMaxLength(2);

            builder.Property(x => x.Quantity)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Panel)
                .HasMaxLength(10);

            builder.Property(x => x.Length)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.CablesSide1)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.CablesSide2)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Lot1)
                .HasMaxLength(50);

            builder.Property(x => x.Lot2)
                .HasMaxLength(50);

            builder.Property(x => x.Lot3)
                .HasMaxLength(50);

            // Propiedades calculadas: no se almacenan en la tabla.
            builder.Ignore(x => x.Sides);
            builder.Ignore(x => x.TotalLength);
            builder.Ignore(x => x.TotalCables);
            builder.Ignore(x => x.Cuts);

            // Auditoría.
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedAt);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            // Concurrencia optimista.
            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            // Relación con la cabecera.
            builder.HasOne(x => x.HojaCorte)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => new
                {
                    x.Year,
                    x.HojaCorteCode
                })
                .HasPrincipalKey(x => new
                {
                    x.Year,
                    x.Code
                })
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con Producto mediante clave compuesta.
            builder.HasOne(x => x.Producto)
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.ProductoTipoCode,
                    x.ProductoCode
                })
                .HasPrincipalKey(x => new
                {
                    x.TipoProductoCode,
                    x.Code
                })
                .OnDelete(DeleteBehavior.Restrict);

            // Unidad de medida.
            builder.HasOne(x => x.Unit)
                .WithMany()
                .HasForeignKey(x => x.UnitCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}