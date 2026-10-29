using Cepheus.Domain.Comun;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Comun
{
    public class NegocioConfiguration : IEntityTypeConfiguration<Negocio>
    {
        public void Configure(EntityTypeBuilder<Negocio> builder)
        {
            builder.ToTable("Negocios", schema: "comun");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

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