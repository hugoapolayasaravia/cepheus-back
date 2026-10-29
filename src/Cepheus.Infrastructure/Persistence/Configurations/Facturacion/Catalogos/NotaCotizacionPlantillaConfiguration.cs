using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Catalogos
{
    public class NotaCotizacionPlantillaConfiguration : IEntityTypeConfiguration<NotaCotizacionPlantilla>
    {
        public void Configure(EntityTypeBuilder<NotaCotizacionPlantilla> builder)
        {
            builder.ToTable("NotasCotizacionPlantilla", schema: "facturacion");

            builder.HasKey(x => new { x.NegocioCode, x.Code });

            builder.Property(x => x.NegocioCode)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.Option)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(OpcionNotaCotizacion.Observacion)
                .HasSentinel(default(OpcionNotaCotizacion));

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Negocio)
                .WithMany()
                .HasForeignKey(x => x.NegocioCode)
                .HasPrincipalKey(n => n.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.Option);
        }
    }
}
