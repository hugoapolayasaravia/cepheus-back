using Cepheus.Domain.Comunes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Comunes
{
    public class PlantaConfiguration : IEntityTypeConfiguration<Planta>
    {
        public void Configure(EntityTypeBuilder<Planta> builder)
        {
            builder.ToTable("Plantas", schema: "comun");

            builder.HasKey(x => x.Code);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasColumnType("char(2)");

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LegalName)
                .HasMaxLength(150);

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.AddressComplement)
                .HasMaxLength(150);

            builder.Property(x => x.UbigeoCode)
                .HasMaxLength(6);

            builder.Property(x => x.ManagerName)
                .HasMaxLength(100);

            builder.Property(x => x.StatusCode)
                .HasMaxLength(2);

            builder.Property(x => x.HasWarehouse)
                .IsRequired();

            builder.Property(x => x.IsProductionPlant)
                .IsRequired();

            builder.Property(x => x.IsProject)
                .IsRequired();

            builder.Property(x => x.RequiresApprovals)
                .IsRequired();

            builder.Property(x => x.AppliesDetraction)
                .IsRequired();

            // Numeración de documentos emitidos por la planta
            builder.Property(x => x.NumeroGve)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("001-000000");

            builder.Property(x => x.NumeroFve)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("001-000000");

            builder.Property(x => x.NumeroDve)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("001-000000");

            builder.Property(x => x.NumeroBve)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("001-000000");

            builder.Property(x => x.NumeroCve)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("001-000000");

            builder.Property(x => x.NumeroLet)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("001-000000");

            builder.Property(x => x.NumeroRet)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("001-000000");

            builder.Property(x => x.GuiaNum)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("001-000000");

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