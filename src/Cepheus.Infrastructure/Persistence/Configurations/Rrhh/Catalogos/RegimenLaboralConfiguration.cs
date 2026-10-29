using Cepheus.Domain.Rrhh.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Catalogos
{
    public class RegimenLaboralConfiguration : IEntityTypeConfiguration<RegimenLaboral>
    {
        public void Configure(EntityTypeBuilder<RegimenLaboral> builder)
        {
            builder.ToTable("RegimenesLaborales", schema: "rrhh");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(3)");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            // El script solo define UNIQUE(codigo); no hay UNIQUE(nombre) en rrhh.regimen_laboral.

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}