using Cepheus.Domain.Comunes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Comunes
{
    public class ControlVentasConfiguration : IEntityTypeConfiguration<ControlVentas>
    {
        public void Configure(EntityTypeBuilder<ControlVentas> builder)
        {
            builder.ToTable("ControlesVentas", schema: "comun");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.IgvPercentage).HasColumnType("numeric(24,2)").IsRequired();
            builder.Property(x => x.WithholdingPercentage).HasColumnType("numeric(24,2)").IsRequired();

            builder.Property(x => x.ClosingPeriod)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(x => x.SalesProcessDate).IsRequired();
            builder.Property(x => x.PurchasesProcessDate).IsRequired();
            builder.Property(x => x.SalesCancelDate).IsRequired();
            builder.Property(x => x.PurchasesCancelDate).IsRequired();

            builder.Property(x => x.WithholdingCap).HasColumnType("numeric(12,2)").IsRequired();
            builder.Property(x => x.DetractionPercentage).HasColumnType("numeric(8,2)").IsRequired();
            builder.Property(x => x.IncomeTaxPercentage).HasColumnType("numeric(5,2)").IsRequired();
            builder.Property(x => x.FonaviPercentage).HasColumnType("numeric(5,2)").IsRequired();
            builder.Property(x => x.ForeignIgvPercentage).HasColumnType("numeric(5,2)").IsRequired();
            builder.Property(x => x.QuotaPercentage).HasColumnType("numeric(6,2)").IsRequired();

            builder.Property(x => x.BlocksGrouping).IsRequired();
            builder.Property(x => x.WithholdingCapInvoice).HasColumnType("numeric(12,2)").IsRequired();

            builder.Property(x => x.WorkOrderDaysLimit).IsRequired();
            builder.Property(x => x.WorkOrderMaxDays).IsRequired();
            builder.Property(x => x.SaleMaxDays).IsRequired();
            builder.Property(x => x.BalanceLimit);
            builder.Property(x => x.AdditionalActivationDays);

            builder.Property(x => x.IsServiceIndicator).IsRequired();

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
