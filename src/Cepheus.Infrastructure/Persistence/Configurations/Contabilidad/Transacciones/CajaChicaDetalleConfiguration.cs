using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Transacciones
{
    public class CajaChicaDetalleConfiguration : IEntityTypeConfiguration<CajaChicaDetalle>
    {
        public void Configure(EntityTypeBuilder<CajaChicaDetalle> builder)
        {
            builder.ToTable("CajasChicaDetalle", schema: "contabilidad");

            builder.HasKey(x => new { x.CajaCode, x.ItemNumber });

            builder.Property(x => x.CajaCode).IsRequired().HasColumnType("char(6)");
            builder.Property(x => x.ItemNumber).IsRequired();

            builder.Property(x => x.Origen).IsRequired().HasConversion<int>();
            builder.Property(x => x.ReferenceCode).HasColumnType("char(6)");
            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");

            builder.Property(x => x.ComprobantePagoCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.NumeroDocumento).IsRequired().HasMaxLength(15);
            builder.Property(x => x.FechaEmision).IsRequired();

            builder.Property(x => x.MonedaCode).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.TipoCambio).IsRequired().HasColumnType("decimal(18,4)");

            builder.Property(x => x.Cantidad).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.PrecioUnidad).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(x => x.Parcial).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.Igv).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.NoGravable).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.Renta).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.Fonavi).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.Servicio).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.IgvExterior).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.Total).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(x => x.CuentaContableCode).IsRequired().HasColumnType("char(8)");
            builder.Property(x => x.AnexoTipoCode).HasColumnType("char(3)");
            builder.Property(x => x.AnexoCode).HasColumnType("char(5)");
            builder.Property(x => x.Concepto).HasMaxLength(100);

            builder.Property(x => x.ProveedorCode).IsRequired().HasColumnType("char(5)");

            builder.Property(x => x.DocumentoReferenciaComprobantePagoCode).HasColumnType("char(2)");
            builder.Property(x => x.DocumentoReferenciaNumero).HasMaxLength(10);

            builder.HasOne(x => x.Planta).WithMany().HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(p => p.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ComprobantePago).WithMany().HasForeignKey(x => x.ComprobantePagoCode)
                .HasPrincipalKey(c => c.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode)
                .HasPrincipalKey(m => m.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CuentaContable).WithMany().HasForeignKey(x => x.CuentaContableCode)
                .HasPrincipalKey(c => c.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Anexo).WithMany()
                .HasForeignKey(x => new { x.AnexoTipoCode, x.AnexoCode })
                .HasPrincipalKey(a => new { a.TipoAnexoCode, a.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Proveedor).WithMany().HasForeignKey(x => x.ProveedorCode)
                .HasPrincipalKey(p => p.Code).OnDelete(DeleteBehavior.Restrict);

            // Un comprobante no puede estar en dos cajas (el tipo 00 = "sin documento" se excluye).
            builder.HasIndex(x => new { x.ComprobantePagoCode, x.NumeroDocumento, x.ProveedorCode })
                .IsUnique()
                .HasFilter("[ComprobantePagoCode] <> '00'");

            builder.HasIndex(x => new { x.Origen, x.PlantaCode, x.ReferenceCode });
        }
    }
}
