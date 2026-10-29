using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorJornadaConfiguration : IEntityTypeConfiguration<TrabajadorJornada>
    {
        public void Configure(EntityTypeBuilder<TrabajadorJornada> builder)
        {
            builder.ToTable("trabajador_jornada", schema: "rrhh");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.TrabajadorCode).IsRequired();
            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:1 con Trabajador (igual que el legacy: UNIQUE(trabajador_id))
            builder.HasIndex(x => x.TrabajadorCode).IsUnique();

            builder.HasOne(x => x.Horario).WithMany().HasForeignKey(x => x.HorarioCode).OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.HorasExtras).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.HorasExt40).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.HorasExtCon).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.HorasExtCon125).IsRequired().HasColumnType("decimal(15,2)").HasDefaultValue(0);
            builder.Property(x => x.HorasExtCon135).IsRequired().HasColumnType("decimal(15,2)").HasDefaultValue(0);
            builder.Property(x => x.ControlHorario).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.HorarioOrdinario).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.HorarioNocturno).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.JornadaMaxima).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.RegimenAlternativo).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
