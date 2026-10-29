using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorConfiguration : IEntityTypeConfiguration<Trabajador>
    {
        public void Configure(EntityTypeBuilder<Trabajador> builder)
        {
            builder.ToTable("Trabajadores", schema: "rrhh");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(5)");

            builder.Property(x => x.FirstNames)
                .HasMaxLength(60);

            builder.Property(x => x.PaternalSurname)
                .HasMaxLength(60);

            builder.Property(x => x.MaternalSurname)
                .HasMaxLength(60);

            builder.Property(x => x.SexoCode)
                .HasMaxLength(10);

            builder.Property(x => x.EstadoCivilCode)
                .HasMaxLength(10);

            builder.Property(x => x.NacionalidadCode)
                .HasMaxLength(10);

            builder.Property(x => x.BirthDate)
                .HasColumnType("date");

            builder.Property(x => x.BirthUbigeoCode)
                .HasMaxLength(6);

            builder.Property(x => x.Email)
                .HasMaxLength(150);

            builder.Property(x => x.Phone)
                .HasMaxLength(25);

            builder.Property(x => x.MobilePhone)
                .HasMaxLength(25);

            builder.Property(x => x.PhotoUrl)
                .HasMaxLength(250);

            builder.Property(x => x.HasDisability)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.Sexo)
                .WithMany()
                .HasForeignKey(x => x.SexoCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.EstadoCivil)
                .WithMany()
                .HasForeignKey(x => x.EstadoCivilCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Nacionalidad)
                .WithMany()
                .HasForeignKey(x => x.NacionalidadCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BirthUbigeo)
                .WithMany()
                .HasForeignKey(x => x.BirthUbigeoCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}