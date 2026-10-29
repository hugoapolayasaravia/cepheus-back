using Cepheus.Domain.Comunes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Comunes
{
    public class PeriodoIgvConfiguration : IEntityTypeConfiguration<PeriodoIgv>
    {
        public void Configure(EntityTypeBuilder<PeriodoIgv> builder)
        {
            builder.ToTable("PeriodosIgv", schema: "comun");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FechaInicio)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(x => x.FechaFin)
                .HasColumnType("date");

            // Un período por fecha de inicio. El no-solapamiento se valida en la aplicación (no se puede
            // expresar de forma portable entre motores con un índice).
            builder.HasIndex(x => x.FechaInicio).IsUnique();

            builder.Property(x => x.PorcentajeIgv)
                .IsRequired()
                .HasColumnType("numeric(5,2)");

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
