using Cepheus.Domain.Logistica.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Catalogos
{
    public class UnidadNegocioConfiguration : IEntityTypeConfiguration<UnidadNegocio>
    {
        public void Configure(EntityTypeBuilder<UnidadNegocio> builder)
        {
            builder.ToTable("UnidadesNegocio", schema: "logistica");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(6)");

            builder.Property(x => x.Name)
                .HasMaxLength(50);

            builder.Property(x => x.ParentCode)
                .HasMaxLength(6);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            // Auto-referencia: unidad padre / unidades hijas.
            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentCode)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}