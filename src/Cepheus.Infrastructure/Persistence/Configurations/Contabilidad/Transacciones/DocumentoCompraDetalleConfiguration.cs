using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Transacciones
{
    public class DocumentoCompraDetalleConfiguration : IEntityTypeConfiguration<DocumentoCompraDetalle>
    {
        public void Configure(EntityTypeBuilder<DocumentoCompraDetalle> builder)
        {
            builder.ToTable("DocumentoCompraDetalles", schema: "contabilidad");

            builder.HasKey(x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode, x.ItemNumber });

            builder.Property(x => x.ComprobantePagoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.NumeroDocumento).IsRequired().HasColumnType("char(15)");
            builder.Property(x => x.ProveedorCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.ItemNumber).IsRequired();

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.HasOne(x => x.Planta).WithMany().HasForeignKey(x => x.PlantaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CuentaContableCode).IsRequired().HasColumnType("char(8)");
            builder.HasOne(x => x.CuentaContable).WithMany().HasForeignKey(x => x.CuentaContableCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.AnexoTipoCode).HasColumnType("char(3)");
            builder.Property(x => x.AnexoCode).HasColumnType("char(5)");
            builder.HasOne(x => x.Anexo).WithMany()
                .HasForeignKey(x => new { x.AnexoTipoCode, x.AnexoCode })
                .HasPrincipalKey(a => new { a.TipoAnexoCode, a.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Detalle).IsRequired().HasMaxLength(50);

            builder.Property(x => x.Cantidad).IsRequired().HasColumnType("decimal(24,4)").HasDefaultValue(0);

            builder.Property(x => x.UnidadMedidaCode).IsRequired().HasColumnType("char(2)");
            builder.HasOne(x => x.UnidadMedida).WithMany().HasForeignKey(x => x.UnidadMedidaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.PrecioSol).IsRequired().HasColumnType("decimal(24,5)").HasDefaultValue(0);
            builder.Property(x => x.ParcialSol).IsRequired().HasColumnType("decimal(24,2)").HasDefaultValue(0);

            builder.Property(x => x.NumeroGuiaVenta).HasColumnType("char(10)");

            builder.Property(x => x.ProductoTipoCode).HasColumnType("char(2)");
            builder.Property(x => x.ProductoCode).HasColumnType("char(4)");
            builder.HasOne(x => x.Producto).WithMany()
                .HasForeignKey(x => new { x.ProductoTipoCode, x.ProductoCode })
                .HasPrincipalKey(p => new { p.TipoProductoCode, p.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
