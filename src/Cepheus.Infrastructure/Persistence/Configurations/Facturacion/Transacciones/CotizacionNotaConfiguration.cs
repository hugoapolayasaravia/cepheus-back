using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class CotizacionNotaConfiguration : IEntityTypeConfiguration<CotizacionNota>
    {
        public void Configure(EntityTypeBuilder<CotizacionNota> builder)
        {
            builder.ToTable("CotizacionesNotas", schema: "facturacion");

            builder.HasKey(x => new { x.NegocioCode, x.Year, x.Month, x.Code, x.Sequence });

            builder.Property(x => x.NegocioCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Year).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.Month).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(8)");

            builder.Property(x => x.Sequence).ValueGeneratedOnAdd();

            builder.Property(x => x.Description).IsRequired();

            builder.Property(x => x.Option)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(OpcionNotaCotizacion.Observacion)
                .HasSentinel(default(OpcionNotaCotizacion));

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Cotizacion).WithMany(c => c.Notas)
                .HasForeignKey(x => new { x.NegocioCode, x.Year, x.Month, x.Code })
                .HasPrincipalKey(c => new { c.NegocioCode, c.Year, c.Month, c.Code })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
