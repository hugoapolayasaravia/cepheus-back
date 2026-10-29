using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class NotaCreditoVentaConfiguration : IEntityTypeConfiguration<NotaCreditoVenta>
    {
        public void Configure(EntityTypeBuilder<NotaCreditoVenta> builder)
        {
            builder.ToTable("NotasCreditoVenta", schema: "facturacion");

            builder.HasKey(x => new { x.PlantaCode, x.Number });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Number).IsRequired().HasMaxLength(10);
            builder.Property(x => x.IssueDate).IsRequired();

            builder.Property(x => x.ClienteCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.ObraCode).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.VendedorCode).HasColumnType("char(4)");

            builder.Property(x => x.DeliveryAddress).IsRequired().HasMaxLength(100);
            builder.Property(x => x.DeliveryUbigeoCode).IsRequired().HasColumnType("char(6)");

            builder.Property(x => x.IsPrinted).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.FormaPagoVentaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.MonedaCode).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.ExchangeRate).IsRequired().HasColumnType("decimal(18,4)");

            builder.Property(x => x.OrderCode).HasColumnType("char(8)");
            builder.Property(x => x.Description).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ExecutingPlantaCode).IsRequired().HasColumnType("char(2)");

            builder.Property(x => x.SubTotal).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.IgvAmount).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.TotalAmount).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.PendingAmount).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.Property(x => x.ProvisionEntryNumber).HasColumnType("char(13)");
            builder.Property(x => x.CancellationEntryNumber).HasColumnType("char(13)");
            builder.Property(x => x.CancelReason).HasMaxLength(500);

            builder.Property(x => x.Type).IsRequired().HasColumnType("char(1)").HasDefaultValue("N");

            builder.Property(x => x.DocumentTypeCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.DocumentNumber).IsRequired().HasMaxLength(10);
            builder.Property(x => x.MotivoDevolucionCode).IsRequired().HasColumnType("char(2)");

            builder.Property(x => x.SolicitudCode).HasColumnType("char(8)");
            builder.Property(x => x.SolicitudYear).HasColumnType("char(4)");
            builder.Property(x => x.SolicitudMonth).HasColumnType("char(2)");

            builder.Property(x => x.CashDocument).HasMaxLength(10);

            builder.Property(x => x.Status).IsRequired().HasConversion<int>()
                .HasDefaultValue(EstadoNotaCredito.Pendiente)
                .HasSentinel(default(EstadoNotaCredito));

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Relaciones
            builder.HasOne(x => x.Planta).WithMany().HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(p => p.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ExecutingPlanta).WithMany().HasForeignKey(x => x.ExecutingPlantaCode)
                .HasPrincipalKey(p => p.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteCode)
                .HasPrincipalKey(c => c.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Obra).WithMany()
                .HasForeignKey(x => new { x.ClienteCode, x.ObraCode })
                .HasPrincipalKey(o => new { o.ClienteCode, o.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Vendedor).WithMany().HasForeignKey(x => x.VendedorCode)
                .HasPrincipalKey(v => v.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DeliveryUbigeo).WithMany().HasForeignKey(x => x.DeliveryUbigeoCode)
                .HasPrincipalKey(u => u.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FormaPagoVenta).WithMany().HasForeignKey(x => x.FormaPagoVentaCode)
                .HasPrincipalKey(f => f.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Moneda).WithMany().HasForeignKey(x => x.MonedaCode)
                .HasPrincipalKey(m => m.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DocumentType).WithMany().HasForeignKey(x => x.DocumentTypeCode)
                .HasPrincipalKey(c => c.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.MotivoDevolucion).WithMany().HasForeignKey(x => x.MotivoDevolucionCode)
                .HasPrincipalKey(m => m.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Solicitud).WithMany()
                .HasForeignKey(x => new { x.SolicitudCode, x.SolicitudYear, x.SolicitudMonth })
                .HasPrincipalKey(s => new { s.Code, s.Year, s.Month })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Details).WithOne(d => d.NotaCredito)
                .HasForeignKey(d => new { d.PlantaCode, d.Number })
                .HasPrincipalKey(n => new { n.PlantaCode, n.Number })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ClienteCode);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.IssueDate);
            builder.HasIndex(x => new { x.SolicitudCode, x.SolicitudYear, x.SolicitudMonth });
        }
    }
}
