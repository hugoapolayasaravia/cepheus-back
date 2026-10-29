using Cepheus.Domain.Contabilidad.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Catalogos
{
    public class TipoAnexoConfiguration : IEntityTypeConfiguration<TipoAnexo>
    {
        public void Configure(EntityTypeBuilder<TipoAnexo> builder)
        {
            builder.ToTable("TiposAnexo", schema: "contabilidad");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(3)");

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

            builder.Property(x => x.Flag).HasColumnType("char(1)");

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
