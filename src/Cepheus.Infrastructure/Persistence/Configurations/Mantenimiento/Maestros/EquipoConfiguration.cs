using Cepheus.Domain.Mantenimiento.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Mantenimiento.Maestros
{
    public class EquipoConfiguration : IEntityTypeConfiguration<Equipo>
    {
        public void Configure(EntityTypeBuilder<Equipo> builder)
        {
            builder.ToTable("Equipos", schema: "mantenimiento");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                 .HasColumnType("char(8)");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Nivel)
                .IsRequired();

            builder.Property(x => x.SubCentroCostoCode)
                .HasMaxLength(6);

            builder.HasOne(x => x.SubCentroCosto)
                .WithMany()
                .HasForeignKey(x => x.SubCentroCostoCode)
                .OnDelete(DeleteBehavior.Restrict);

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
