// Cepheus.Infrastructure/Persistence/Configurations/Logistica/Transacciones/GuiaConfiguration.cs
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones
{
    public class GuiaConfiguration : IEntityTypeConfiguration<Guia>
    {
        public void Configure(EntityTypeBuilder<Guia> builder)
        {
            builder.ToTable("Guias", schema: "logistica");

            builder.HasKey(x => new { x.PlantaCode, x.Code });

            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.HasOne(x => x.Planta)
                .WithMany()
                .HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            // 'SSS-NNNNNN' (serie + guion + correlativo)
            builder.Property(x => x.Code).IsRequired().HasColumnType("char(10)");

            builder.Property(x => x.FechaEmision).IsRequired().HasColumnType("date");
            builder.Property(x => x.Hora).IsRequired().HasColumnType("char(5)");

            builder.Property(x => x.ProveedorCode).IsRequired().HasColumnType("char(5)");
            builder.HasOne(x => x.Proveedor)
                .WithMany()
                .HasForeignKey(x => x.ProveedorCode)
                .OnDelete(DeleteBehavior.Restrict);

            // El catálogo de motivos es Comunes.MotivoDevolucion (PK = Id, Code
            // es único), por eso la FK apunta a la clave alterna Code.
            builder.Property(x => x.MotivoCode).IsRequired().HasColumnType("char(2)");
            builder.HasOne(x => x.Motivo)
                .WithMany()
                .HasForeignKey(x => x.MotivoCode)
                .HasPrincipalKey(x => x.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Direccion).IsRequired().HasMaxLength(70);
            builder.Property(x => x.PuntoPartida).IsRequired().HasMaxLength(70);

            builder.Property(x => x.TransportistaCode).IsRequired().HasColumnType("char(5)");
            builder.HasOne(x => x.Transportista)
                .WithMany()
                .HasForeignKey(x => x.TransportistaCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ConductorCode).IsRequired().HasColumnType("char(5)");
            builder.HasOne(x => x.Conductor)
                .WithMany()
                .HasForeignKey(x => x.ConductorCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.VehiculoCode).IsRequired().HasColumnType("char(5)");
            builder.HasOne(x => x.Vehiculo)
                .WithMany()
                .HasForeignKey(x => x.VehiculoCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Observaciones).IsRequired().HasMaxLength(50).HasDefaultValue(string.Empty);

            builder.Property(x => x.Estado).IsRequired().HasConversion<int>();

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.Property(x => x.RowVersion).IsRowVersion();

            // Búsqueda por rango de fechas dentro de una planta (listado legacy).
            builder.HasIndex(x => new { x.PlantaCode, x.FechaEmision });

            builder.HasMany(x => x.Detalles)
                .WithOne(x => x.Guia)
                .HasForeignKey(x => new { x.PlantaCode, x.GuiaCode })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
