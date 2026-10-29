using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorDocumentoConfiguration : IEntityTypeConfiguration<TrabajadorDocumento>
    {
        public void Configure(EntityTypeBuilder<TrabajadorDocumento> builder)
        {
            builder.ToTable("TrabajadorDocumentos", schema: "rrhh");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TrabajadorCode)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(x => x.TipoDocumentoCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.DocumentNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TipoDocumento)
                .WithMany()
                .HasForeignKey(x => x.TipoDocumentoCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.TrabajadorCode);

            builder.HasIndex(x => new { x.TipoDocumentoCode, x.DocumentNumber })
                .IsUnique();
        }
    }
}