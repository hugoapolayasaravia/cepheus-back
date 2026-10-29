using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Cepheus.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class OrdenTrabajoVentaAprobacionConfiguration : IEntityTypeConfiguration<OrdenTrabajoVentaAprobacion>
    {
        public void Configure(EntityTypeBuilder<OrdenTrabajoVentaAprobacion> builder)
        {
            builder.ToTable("OrdenesTrabajoVentaAprobaciones", schema: "facturacion");

            builder.HasKey(x => x.OrdenTrabajoCode);

            builder.Property(x => x.OrdenTrabajoCode).IsRequired().HasColumnType("char(8)");

            builder.Property(x => x.CreditsObservations).HasMaxLength(400);

            builder.Property(x => x.Document1Number).HasColumnType("char(10)");
            builder.Property(x => x.Document2Number).HasColumnType("char(10)");

            builder.Property(x => x.DocumentType)
                .IsRequired()
                .HasColumnType("char(1)")
                .HasConversion(new LegacyCharConverter<TipoDocumentoAprobacion>(
                    (TipoDocumentoAprobacion.Factura, "F"),
                    (TipoDocumentoAprobacion.Boleta, "B")));

            builder.Property(x => x.PaymentType)
                .IsRequired()
                .HasColumnType("char(1)")
                .HasConversion(new LegacyCharConverter<TipoPagoAprobacion>(
                    (TipoPagoAprobacion.Credito, "R"),
                    (TipoPagoAprobacion.Contado, "C")));

            builder.Property(x => x.CreditStatus)
                .IsRequired()
                .HasColumnType("char(1)")
                .HasConversion(new LegacyCharConverter<EstadoCreditoAprobacion>(
                    (EstadoCreditoAprobacion.Aprobado, "A"),
                    (EstadoCreditoAprobacion.Rechazado, "R")));

            builder.Property(x => x.DocumentFlow)
                .IsRequired()
                .HasColumnType("char(1)")
                .HasConversion(new LegacyCharConverter<FlujoDocumentoAprobacion>(
                    (FlujoDocumentoAprobacion.SinDocumento, "N"),
                    (FlujoDocumentoAprobacion.TipoP, "P"),
                    (FlujoDocumentoAprobacion.TipoG, "G"),
                    (FlujoDocumentoAprobacion.TipoC, "C")));

            builder.Property(x => x.ApprovedQuantity).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.ApprovedAt).IsRequired();

            builder.Property(x => x.ValorizationCode).IsRequired().HasColumnType("char(1)").HasDefaultValue("N");
            builder.Property(x => x.ValorizationWeekday).HasConversion<int?>();

            builder.Property(x => x.HasTwoDocuments).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.Plant1Code).HasColumnType("char(2)");
            builder.Property(x => x.Plant2Code).HasColumnType("char(2)");
            builder.Property(x => x.GuideMotivoCode).HasColumnType("char(2)");
            builder.Property(x => x.PaymentMethodCode).HasColumnType("char(2)");
            builder.Property(x => x.BankCode).HasColumnType("char(3)");
            builder.Property(x => x.PaymentDocumentNumber).HasMaxLength(50);
            builder.Property(x => x.FormaPagoVentaCode).IsRequired().HasColumnType("char(2)");

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Plant1).WithMany()
                .HasForeignKey(x => x.Plant1Code).HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Plant2).WithMany()
                .HasForeignKey(x => x.Plant2Code).HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Bank).WithMany()
                .HasForeignKey(x => x.BankCode).HasPrincipalKey(b => b.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FormaPagoVenta).WithMany()
                .HasForeignKey(x => x.FormaPagoVentaCode).HasPrincipalKey(f => f.Code)
                .OnDelete(DeleteBehavior.Restrict);

            // Un documento de pago no se asigna a dos OT (PB: ue_validardocumento).
            builder.HasIndex(x => new { x.DocumentType, x.Document1Number });
            builder.HasIndex(x => new { x.DocumentType, x.Document2Number });
        }
    }
}
