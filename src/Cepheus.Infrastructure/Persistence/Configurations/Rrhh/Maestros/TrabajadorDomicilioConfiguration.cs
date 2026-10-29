using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Rrhh.Maestros
{
    public class TrabajadorDomicilioConfiguration : IEntityTypeConfiguration<TrabajadorDomicilio>
    {
        public void Configure(EntityTypeBuilder<TrabajadorDomicilio> builder)
        {
            builder.ToTable("TrabajadorDomicilios", schema: "rrhh");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TrabajadorCode)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(x => x.RoadTypeCode)
                .HasMaxLength(10);

            builder.Property(x => x.StreetName)
                .HasMaxLength(150);

            builder.Property(x => x.StreetNumber)
                .HasMaxLength(20);

            builder.Property(x => x.InteriorNumber)
                .HasMaxLength(20);

            builder.Property(x => x.ZoneTypeCode)
                .HasMaxLength(10);

            builder.Property(x => x.ZoneName)
                .HasMaxLength(150);

            builder.Property(x => x.Reference)
                .HasMaxLength(250);

            builder.Property(x => x.UbigeoCode)
                .HasMaxLength(6);

            builder.Property(x => x.IsPrimary)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(250);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(250);

            builder.HasOne(x => x.Trabajador)
                .WithMany()
                .HasForeignKey(x => x.TrabajadorCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.RoadType)
                .WithMany()
                .HasForeignKey(x => x.RoadTypeCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ZoneType)
                .WithMany()
                .HasForeignKey(x => x.ZoneTypeCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Ubigeo)
                .WithMany()
                .HasForeignKey(x => x.UbigeoCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.TrabajadorCode);
        }
    }
}