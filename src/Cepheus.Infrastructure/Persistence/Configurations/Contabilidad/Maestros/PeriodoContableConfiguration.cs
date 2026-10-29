using Cepheus.Domain.Contabilidad.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Maestros
{
    public class PeriodoContableConfiguration : IEntityTypeConfiguration<PeriodoContable>
    {
        public void Configure(EntityTypeBuilder<PeriodoContable> builder)
        {
            builder.ToTable("PeriodosContables", schema: "contabilidad");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code).IsRequired().HasColumnType("char(6)");

            builder.Property(x => x.Year).IsRequired();
            builder.Property(x => x.Month).IsRequired();
            builder.HasIndex(x => new { x.Year, x.Month }).IsUnique();

            builder.Property(x => x.Status).IsRequired().HasConversion<int>();
            builder.HasIndex(x => x.Status);

            builder.Property(x => x.ClosedBy).HasMaxLength(250);
            builder.Property(x => x.ReopenedBy).HasMaxLength(250);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
