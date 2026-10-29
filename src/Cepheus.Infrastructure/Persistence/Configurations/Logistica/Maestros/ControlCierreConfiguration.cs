using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Maestros
{
    public class ControlCierreConfiguration : IEntityTypeConfiguration<ControlCierre>
    {
        public void Configure(EntityTypeBuilder<ControlCierre> builder)
        {
            builder.ToTable("ControlCierres", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.PeriodCode });

            builder.Property(x => x.PlantaCode)
                .IsRequired()
                 .HasColumnType("char(2)");

            builder.Property(x => x.PeriodCode)
                .IsRequired()
                 .HasColumnType("char(6)");

            builder.Property(x => x.ClosureDate)
                .IsRequired();

            builder.Property(x => x.DifferenceAmount)
                .IsRequired()
                .HasColumnType("decimal(18,6)")
                .HasDefaultValue(0);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.HasOne(x => x.Planta)
                .WithMany()
                .HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
