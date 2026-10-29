using Cepheus.Domain.Logistica.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Catalogos
{
    public class FamiliaConfiguration : IEntityTypeConfiguration<Familia>
    {
        public void Configure(EntityTypeBuilder<Familia> builder)
        {
            builder.ToTable("Familias", schema: "logistica");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(2)"); ;

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(60);

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