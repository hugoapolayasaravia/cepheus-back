using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public sealed class NotaIngresoConfiguration : IEntityTypeConfiguration<NotaIngreso>
{
    public void Configure(EntityTypeBuilder<NotaIngreso> builder)
    {
        builder.ToTable("NotaIngresos", "logistica");

        builder.HasKey(x => new { x.PlantaCode, x.Code });

        builder.Property(x => x.PlantaCode).HasColumnType("char(2)").IsRequired();
        builder.Property(x => x.Code).HasColumnType("char(6)").IsRequired();
        builder.HasOne(x => x.Planta)
            .WithMany()
            .HasForeignKey(x => x.PlantaCode)
            .OnDelete(DeleteBehavior.Restrict);

        // Enums persistidos como ordinal (mismo criterio que OC / Pedido).
        builder.Property(x => x.Condicion).HasConversion<int>().IsRequired();
        builder.Property(x => x.Origen).HasConversion<int>().IsRequired();
        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();

        builder.Property(x => x.OrdenCompraCode).HasColumnType("char(6)");
        builder.HasOne(x => x.OrdenCompra)
            .WithMany()
            .HasForeignKey(x => new { x.PlantaCode, x.OrdenCompraCode })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ImportacionCode).HasColumnType("char(6)");
        builder.HasOne(x => x.Importacion)
            .WithMany()
            .HasForeignKey(x => new { x.PlantaCode, x.ImportacionCode })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.PlantaCode, x.ImportacionCode });

        // Comprobante de pago y motivo de devolución: Comunes, FK por Id (igual que la OC).
        builder.HasOne(x => x.ComprobantePago)
            .WithMany()
            .HasForeignKey(x => x.ComprobantePagoCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ComprobantePagoReferenciaCode)
                .HasColumnType("char(2)")
                .IsFixedLength()
                .IsRequired(false);

        builder.HasOne(x => x.ComprobantePagoReferencia)
            .WithMany()
            .HasForeignKey(x => x.ComprobantePagoReferenciaCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.MotivoDevolucionCode)
            .HasMaxLength(2)
            .IsRequired(false);

        builder.HasOne(x => x.MotivoDevolucion)
            .WithMany()
            .HasForeignKey(x => x.MotivoDevolucionCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ProveedorCode).HasColumnType("char(5)").IsRequired();
        builder.HasOne(x => x.Proveedor)
            .WithMany()
            .HasForeignKey(x => x.ProveedorCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.MonedaCode).IsRequired().HasMaxLength(3);
        builder.HasOne(x => x.Moneda)
            .WithMany()
            .HasForeignKey(x => x.MonedaCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.FormaPagoCode).IsRequired().HasMaxLength(2);
        builder.HasOne(x => x.FormaPago)
            .WithMany()
            .HasForeignKey(x => x.FormaPagoCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.NumeroDocumento).HasMaxLength(15);
        builder.Property(x => x.NumeroGuia).HasMaxLength(15);
        builder.Property(x => x.NumeroReferencia).HasMaxLength(15);
        builder.Property(x => x.AsientoContable).HasMaxLength(13);
        builder.Property(x => x.CodigoVale).HasMaxLength(7);

        // Transferencia entre almacenes: el vale pertenece a la planta ORIGEN (otra planta que la de la nota).
        builder.Property(x => x.PlantaOrigenCode).HasColumnType("char(2)");
        builder.Property(x => x.ValeTransferenciaCode).HasColumnType("char(7)");
        builder.HasOne(x => x.ValeTransferencia)
            .WithMany()
            .HasForeignKey(x => new { x.PlantaOrigenCode, x.ValeTransferenciaCode })
            .OnDelete(DeleteBehavior.Restrict);

        // Un vale de transferencia se recibe una sola vez MIENTRAS su nota no esté anulada. No es un índice único
        // porque tras anular la transferencia el vale vuelve a Pendiente y se puede recibir de nuevo (el legacy lo
        // permite). La regla se valida al recibir y la concurrencia la protege el RowVersion del vale.
        builder.HasIndex(x => new { x.PlantaOrigenCode, x.ValeTransferenciaCode })
            .HasDatabaseName("IX_NotaIngresos_ValeTransferencia");

        builder.Property(x => x.FechaRecepcion).IsRequired();
        builder.Property(x => x.FechaEmision).IsRequired();
        builder.Property(x => x.FechaProceso).IsRequired();

        builder.Property(x => x.TipoCambio).HasColumnType("decimal(12,4)").IsRequired();
        builder.Property(x => x.Igv).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Total).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Monto).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.NoGravable).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Renta).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Fonavi).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Servicio).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.IgvExterior).HasColumnType("decimal(12,2)").IsRequired();

        builder.Property(x => x.CreatedBy).HasMaxLength(250);
        builder.Property(x => x.UpdatedBy).HasMaxLength(250);
        builder.Property(x => x.RowVersion).IsRowVersion();

        // Búsquedas del listado legacy (Logi_sp_Listado_MNotaIngresos) y control de duplicados.
        builder.HasIndex(x => new { x.PlantaCode, x.FechaProceso });
        builder.HasIndex(x => new { x.ComprobantePagoCode, x.ProveedorCode, x.NumeroDocumento });

        builder.HasMany(x => x.Detalles)
            .WithOne(x => x.NotaIngreso)
            .HasForeignKey(x => new { x.PlantaCode, x.NotaIngresoCode })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
