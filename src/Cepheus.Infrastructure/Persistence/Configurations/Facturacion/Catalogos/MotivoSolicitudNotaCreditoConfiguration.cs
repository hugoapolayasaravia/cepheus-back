using Cepheus.Domain.Facturacion.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Catalogos
{
    public class MotivoSolicitudNotaCreditoConfiguration : IEntityTypeConfiguration<MotivoSolicitudNotaCredito>
    {
        public void Configure(EntityTypeBuilder<MotivoSolicitudNotaCredito> builder)
        {
            builder.ToTable("MotivosSolicitudNotaCredito", schema: "facturacion");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(80);

            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.ParentCode)
                .HasColumnType("char(2)");

            builder.Property(x => x.MotivoDevolucionCode)
                .HasColumnType("char(2)");

            builder.Property(x => x.AffectsStock)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();

            // Auto-FK opcional (motivo padre / grupo)
            builder.HasOne(x => x.Parent).WithMany()
                .HasForeignKey(x => x.ParentCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            // FK opcional -> Comunes.MotivoDevolucion
            builder.HasOne(x => x.MotivoDevolucion).WithMany()
                .HasForeignKey(x => x.MotivoDevolucionCode)
                .HasPrincipalKey(m => m.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ParentCode);
            builder.HasIndex(x => x.MotivoDevolucionCode);
        }
    }
}
