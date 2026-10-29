using Cepheus.Domain.Contabilidad.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Catalogos
{
    public class TipoAsientoConfiguration : IEntityTypeConfiguration<TipoAsiento>
    {
        public void Configure(EntityTypeBuilder<TipoAsiento> builder)
        {
            builder.ToTable("TiposAsiento", schema: "contabilidad");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(2)");

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

            builder.Property(x => x.ExchangeRateType).IsRequired().HasConversion<int>();

            builder.Property(x => x.Valuation).IsRequired().HasConversion<int>();

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
