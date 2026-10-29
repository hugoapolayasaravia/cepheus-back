using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Cepheus.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class OrdenTrabajoVentaConfiguration : IEntityTypeConfiguration<OrdenTrabajoVenta>
    {
        public void Configure(EntityTypeBuilder<OrdenTrabajoVenta> builder)
        {
            builder.ToTable("OrdenesTrabajoVenta", schema: "facturacion");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code).IsRequired().HasColumnType("char(8)");

            builder.Property(x => x.Type)
                .IsRequired()
                .HasColumnType("char(1)")
                .HasConversion(new LegacyCharConverter<TipoOrdenTrabajoVenta>(
                    (TipoOrdenTrabajoVenta.Normal, "N"),
                    (TipoOrdenTrabajoVenta.Saldo, "S"),
                    (TipoOrdenTrabajoVenta.Anticipo, "A"),
                    (TipoOrdenTrabajoVenta.Descuento, "D"),
                    (TipoOrdenTrabajoVenta.Consolidacion, "C")));

            builder.Property(x => x.Stage)
                .IsRequired()
                .HasColumnType("char(1)")
                .HasConversion(new LegacyCharConverter<EtapaOrdenTrabajoVenta>(
                    (EtapaOrdenTrabajoVenta.Pendiente, "P"),
                    (EtapaOrdenTrabajoVenta.Anulada, "A"),
                    (EtapaOrdenTrabajoVenta.AprobadaCredito, "C"),
                    (EtapaOrdenTrabajoVenta.GeneraProgramacion, "G")));

            builder.Property(x => x.Status).IsRequired().HasConversion<int>();

            builder.Property(x => x.ClienteCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.ObraCode).IsRequired().HasColumnType("char(3)");

            builder.Property(x => x.DispatchDate).IsRequired().HasColumnType("date");
            builder.Property(x => x.DispatchTime).IsRequired().HasColumnType("varchar(5)");

            builder.Property(x => x.VendedorCode).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.CurrencyCode).IsRequired().HasColumnType("char(1)");
            builder.Property(x => x.MotivoCode).IsRequired().HasColumnType("char(2)");

            builder.Property(x => x.Observations).HasMaxLength(50);
            builder.Property(x => x.CreditObservations).IsRequired().HasMaxLength(255).HasDefaultValue("");
            builder.Property(x => x.CutSheetObservations).HasMaxLength(4000);
            builder.Property(x => x.PurchaseOrderNumber).HasMaxLength(20);

            builder.Property(x => x.Total).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.Property(x => x.OriginCode).HasColumnType("char(8)");

            builder.Property(x => x.CotizacionNegocioCode).HasColumnType("char(2)");
            builder.Property(x => x.CotizacionYear).HasColumnType("char(4)");
            builder.Property(x => x.CotizacionMonth).HasColumnType("char(2)");
            builder.Property(x => x.CotizacionCode).HasColumnType("char(8)");

            builder.Property(x => x.IsModifiable).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.ApprovedBy).HasMaxLength(20);
            builder.Property(x => x.ApprovalObservations).HasMaxLength(100);
            builder.Property(x => x.ApprovalDelayReason).HasMaxLength(100);

            builder.Property(x => x.PassedToProduction).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.ProductionPassReason).HasMaxLength(100);

            builder.Property(x => x.SpecialApprovalGranted).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.SpecialApprovalBy).HasMaxLength(20);
            builder.Property(x => x.SpecialApprovalObservations).HasMaxLength(100);

            builder.Property(x => x.CanceledBy).HasMaxLength(20);
            builder.Property(x => x.CancelReason).HasMaxLength(100);

            builder.Property(x => x.InactivatedBy).HasMaxLength(20);
            builder.Property(x => x.InactivationReason).HasMaxLength(100);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Relaciones con maestros y catálogos.
            builder.HasOne(x => x.Cliente).WithMany()
                .HasForeignKey(x => x.ClienteCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Obra).WithMany()
                .HasForeignKey(x => new { x.ClienteCode, x.ObraCode })
                .HasPrincipalKey(o => new { o.ClienteCode, o.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Vendedor).WithMany()
                .HasForeignKey(x => x.VendedorCode)
                .HasPrincipalKey(v => v.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Motivo).WithMany()
                .HasForeignKey(x => x.MotivoCode)
                .HasPrincipalKey(m => m.Code)
                .OnDelete(DeleteBehavior.Restrict);

            // OT origen (Anticipo en el tipo Descuento; OT de origen al copiar).
            builder.HasOne(x => x.Origin).WithMany()
                .HasForeignKey(x => x.OriginCode)
                .HasPrincipalKey(o => o.Code)
                .OnDelete(DeleteBehavior.Restrict);

            // Cotización de origen (opcional, una sola por OT).
            builder.HasOne(x => x.Cotizacion).WithMany()
                .HasForeignKey(x => new { x.CotizacionNegocioCode, x.CotizacionYear, x.CotizacionMonth, x.CotizacionCode })
                .HasPrincipalKey(c => new { c.NegocioCode, c.Year, c.Month, c.Code })
                .OnDelete(DeleteBehavior.Restrict);

            // Hijos del aggregate.
            builder.HasMany(x => x.Detalles).WithOne(d => d.OrdenTrabajo)
                .HasForeignKey(d => d.OrdenTrabajoCode)
                .HasPrincipalKey(o => o.Code)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Aprobacion).WithOne(a => a.OrdenTrabajo)
                .HasForeignKey<OrdenTrabajoVentaAprobacion>(a => a.OrdenTrabajoCode)
                .HasPrincipalKey<OrdenTrabajoVenta>(o => o.Code)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ClienteCode, x.ObraCode });
            builder.HasIndex(x => x.VendedorCode);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.Stage);
            builder.HasIndex(x => x.OriginCode);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
