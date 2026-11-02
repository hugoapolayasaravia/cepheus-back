using Cepheus.Domain.Contabilidad.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Maestros
{
    public class NoAfectoRetencionConfiguration : IEntityTypeConfiguration<NoAfectoRetencion>
    {
        public void Configure(EntityTypeBuilder<NoAfectoRetencion> builder)
        {
            builder.ToTable("NoAfectosRetencion", schema: "contabilidad");

            builder.HasKey(x => new { x.Ruc, x.Type });

            builder.Property(x => x.Ruc).IsRequired().HasColumnType("char(11)");
            builder.Property(x => x.Type).IsRequired().HasColumnType("char(1)");
            builder.Property(x => x.Name).IsRequired().HasMaxLength(70);
            builder.Property(x => x.Resolution).IsRequired().HasMaxLength(50);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
