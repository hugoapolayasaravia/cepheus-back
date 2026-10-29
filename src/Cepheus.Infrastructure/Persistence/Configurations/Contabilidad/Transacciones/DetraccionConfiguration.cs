using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Transacciones
{
    public class DetraccionConfiguration : IEntityTypeConfiguration<Detraccion>
    {
        public void Configure(EntityTypeBuilder<Detraccion> builder)
        {
            builder.ToTable("Detracciones", schema: "contabilidad");

            builder.HasKey(x => new { x.Year, x.Period, x.LotNumber });

            builder.Property(x => x.Year).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Period).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.LotNumber).IsRequired().HasColumnType("char(4)");

            builder.Property(x => x.StartDate).IsRequired().HasColumnType("date");
            builder.Property(x => x.EndDate).IsRequired().HasColumnType("date");
            builder.Property(x => x.DepositDate).HasColumnType("date");
            builder.Property(x => x.CancellationDate).HasColumnType("date");

            builder.Property(x => x.PaymentReceiptNumber).HasColumnType("char(25)");

            builder.Property(x => x.TotalSol).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.Property(x => x.Status).IsRequired().HasConversion<int>()
                .HasDefaultValue(EstadoDetraccion.Pendiente)
                .HasSentinel(default(EstadoDetraccion));

            builder.Property(x => x.AsientoCode).HasColumnType("char(13)");
            builder.Property(x => x.ChequeCode).HasColumnType("char(10)");

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasMany(x => x.Items).WithOne(i => i.Detraccion)
                .HasForeignKey(i => new { i.Year, i.Period, i.LotNumber })
                .HasPrincipalKey(d => new { d.Year, d.Period, d.LotNumber })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
