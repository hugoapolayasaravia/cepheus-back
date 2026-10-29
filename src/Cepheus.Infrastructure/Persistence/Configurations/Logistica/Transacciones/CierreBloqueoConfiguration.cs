using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Logistica.Transacciones;

public class CierreBloqueoConfiguration : IEntityTypeConfiguration<CierreBloqueo>
{
    public void Configure(EntityTypeBuilder<CierreBloqueo> builder)
    {
        builder.ToTable("CierreBloqueos", schema: "logistica");

        builder.HasKey(x => x.PlantaCode);
        builder.Property(x => x.PlantaCode).IsRequired().HasColumnType("char(2)");
        builder.Property(x => x.UltimoUso).IsRequired();
    }
}
