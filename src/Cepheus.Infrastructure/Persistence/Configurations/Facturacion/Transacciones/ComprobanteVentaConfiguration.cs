using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class ComprobanteVentaConfiguration : IEntityTypeConfiguration<ComprobanteVenta>
    {
        public void Configure(EntityTypeBuilder<ComprobanteVenta> builder)
        {
            builder.ToTable("ComprobantesVenta", schema: "facturacion");

            builder.HasKey(x => new { x.TypeCode, x.Number });

            builder.Property(x => x.TypeCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Number).IsRequired().HasColumnType("char(10)");
            builder.Property(x => x.PlantCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.ExecutingPlantCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.IssueDate).IsRequired().HasColumnType("date");

            builder.Property(x => x.ClienteCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.ObraCode).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.VendedorCode).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Address).IsRequired().HasMaxLength(100);
            builder.Property(x => x.UbigeoCode).IsRequired().HasColumnType("char(6)");
            builder.Property(x => x.PaymentFormCode).IsRequired().HasColumnType("char(2)");

            builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(3);
            builder.Property(x => x.ExchangeRate).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(x => x.OrdenTrabajoCode).HasColumnType("char(8)");

            builder.Property(x => x.Origin).IsRequired().HasConversion<int>();
            builder.Property(x => x.DocumentFlow).HasConversion<int?>();
            builder.Property(x => x.Status).IsRequired().HasConversion<int>();

            builder.Property(x => x.Glosa).HasMaxLength(200);
            builder.Property(x => x.IgvPercentage).IsRequired().HasColumnType("decimal(9,2)");
            builder.Property(x => x.SubtotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.IgvAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.TotalSoles).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(x => x.ProvisionEntry).HasColumnType("char(13)");
            builder.Property(x => x.DetractionDocument).HasColumnType("char(10)");
            builder.Property(x => x.DetractionAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.CashDocument).HasColumnType("char(10)");

            builder.Property(x => x.CanceledBy).HasMaxLength(250);
            builder.Property(x => x.CancelReason).HasMaxLength(250);
            builder.Property(x => x.ReplacedByNumber).HasColumnType("char(10)");

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Planta).WithMany()
                .HasForeignKey(x => x.PlantCode).HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Cliente).WithMany()
                .HasForeignKey(x => x.ClienteCode).HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Obra).WithMany()
                .HasForeignKey(x => new { x.ClienteCode, x.ObraCode })
                .HasPrincipalKey(o => new { o.ClienteCode, o.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Vendedor).WithMany()
                .HasForeignKey(x => x.VendedorCode).HasPrincipalKey(v => v.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Ubigeo).WithMany()
                .HasForeignKey(x => x.UbigeoCode).HasPrincipalKey(u => u.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PaymentForm).WithMany()
                .HasForeignKey(x => x.PaymentFormCode).HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OrdenTrabajo).WithMany()
                .HasForeignKey(x => x.OrdenTrabajoCode).HasPrincipalKey(o => o.Code)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Detalles).WithOne(d => d.Comprobante)
                .HasForeignKey(d => new { d.TypeCode, d.Number })
                .HasPrincipalKey(c => new { c.TypeCode, c.Number })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.IssueDate);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.OrdenTrabajoCode);
            builder.HasIndex(x => new { x.ClienteCode, x.ObraCode });
        }
    }
}
