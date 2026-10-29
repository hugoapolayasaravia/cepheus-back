using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Contabilidad.Transacciones
{
    public class CajaChicaConfiguration : IEntityTypeConfiguration<CajaChica>
    {
        public void Configure(EntityTypeBuilder<CajaChica> builder)
        {
            builder.ToTable("CajasChica", schema: "contabilidad");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code).IsRequired().HasColumnType("char(6)");
            builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.TrabajadorCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.IssueDate).IsRequired();
            builder.Property(x => x.Observations).HasMaxLength(200);

            builder.Property(x => x.MontoSol).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(x => x.TotalSol).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            builder.Property(x => x.ProvisionAsientoCode).HasColumnType("char(13)");
            builder.Property(x => x.ReversionAsientoCode).HasColumnType("char(13)");

            builder.Property(x => x.Type).IsRequired().HasConversion<int>();
            builder.Property(x => x.IsPrinted).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.Status).IsRequired().HasConversion<int>()
                .HasDefaultValue(EstadoCajaChica.Pendiente)
                .HasSentinel(default(EstadoCajaChica));

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Planta).WithMany().HasForeignKey(x => x.PlantaCode)
                .HasPrincipalKey(p => p.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Trabajador).WithMany().HasForeignKey(x => x.TrabajadorCode)
                .HasPrincipalKey(t => t.Code).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Details).WithOne(d => d.Caja)
                .HasForeignKey(d => d.CajaCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.PlantaCode);
            builder.HasIndex(x => x.TrabajadorCode);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.IssueDate);
        }
    }
}
