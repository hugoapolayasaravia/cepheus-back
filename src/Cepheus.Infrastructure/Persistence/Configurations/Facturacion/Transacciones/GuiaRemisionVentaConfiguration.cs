using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cepheus.Infrastructure.Persistence.Configurations.Facturacion.Transacciones
{
    public class GuiaRemisionVentaConfiguration : IEntityTypeConfiguration<GuiaRemisionVenta>
    {
        public void Configure(EntityTypeBuilder<GuiaRemisionVenta> builder)
        {
            builder.ToTable("GuiasRemisionVenta", schema: "facturacion");

            builder.HasKey(x => x.Number);

            builder.Property(x => x.Number).IsRequired().HasColumnType("char(10)");
            builder.Property(x => x.PlantCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.IssueDate).IsRequired().HasColumnType("date");
            builder.Property(x => x.DispatchTime).HasColumnType("char(5)");
            builder.Property(x => x.Motive).IsRequired().HasConversion<int>();

            builder.Property(x => x.ClienteCode).IsRequired().HasColumnType("char(5)");
            builder.Property(x => x.ObraCode).IsRequired().HasColumnType("char(3)");
            builder.Property(x => x.VendedorCode).IsRequired().HasColumnType("char(4)");

            builder.Property(x => x.Address).IsRequired().HasMaxLength(200);
            builder.Property(x => x.UbigeoCode).IsRequired().HasColumnType("char(6)");
            builder.Property(x => x.Description).IsRequired().HasMaxLength(250);

            builder.Property(x => x.TransportistaCode).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.ChoferCode).IsRequired().HasColumnType("char(4)");
            builder.Property(x => x.VehicleType).HasConversion<int?>();
            builder.Property(x => x.VehicleCode).HasColumnType("char(4)");

            builder.Property(x => x.Status).IsRequired().HasConversion<int>();

            builder.Property(x => x.DocumentFlow).HasConversion<int?>();
            builder.Property(x => x.DocumentTypeCode).HasColumnType("char(2)");
            builder.Property(x => x.Document1Number).HasColumnType("char(10)");
            builder.Property(x => x.Document2Number).HasColumnType("char(10)");

            builder.Property(x => x.OrdenTrabajoCode).HasColumnType("char(8)");
            builder.Property(x => x.ProgramacionType).HasConversion<int?>();
            builder.Property(x => x.ProgramacionCode).HasColumnType("char(8)");

            builder.Property(x => x.FleteCode).IsRequired().HasColumnType("char(2)");
            builder.Property(x => x.FleteAmount).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(x => x.ReplacesNumber).HasColumnType("char(10)");
            builder.Property(x => x.DevolutionMotiveCode).HasColumnType("char(2)");
            builder.Property(x => x.DevolutionActionCode).HasColumnType("char(2)");

            builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(3);
            builder.Property(x => x.ExchangeRate).IsRequired().HasColumnType("decimal(18,4)");

            builder.Property(x => x.TicketNumber).HasMaxLength(20);
            builder.Property(x => x.PumpCode).HasMaxLength(4);

            builder.Property(x => x.CanceledBy).HasMaxLength(250);
            builder.Property(x => x.CancelReason).HasMaxLength(250);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.Planta).WithMany()
                .HasForeignKey(x => x.PlantCode).HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Cliente).WithMany()
                .HasForeignKey(x => x.ClienteCode).HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Obra).WithMany()
                .HasForeignKey(x => new { x.ClienteCode, x.ObraCode })
                .HasPrincipalKey(o => new { o.ClienteCode, o.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Vendedor).WithMany()
                .HasForeignKey(x => x.VendedorCode).HasPrincipalKey(v => v.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Ubigeo).WithMany()
                .HasForeignKey(x => x.UbigeoCode).HasPrincipalKey(u => u.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Transportista).WithMany()
                .HasForeignKey(x => x.TransportistaCode).HasPrincipalKey(t => t.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Chofer).WithMany()
                .HasForeignKey(x => new { x.TransportistaCode, x.ChoferCode })
                .HasPrincipalKey(c => new { c.TransportistaCode, c.Code })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Vehiculo).WithMany()
                .HasForeignKey(x => new { x.TransportistaCode, x.VehicleType, x.VehicleCode })
                .HasPrincipalKey(v => new { v.TransportistaCode, v.VehicleType, v.Code })
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OrdenTrabajo).WithMany()
                .HasForeignKey(x => x.OrdenTrabajoCode).HasPrincipalKey(o => o.Code)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Flete).WithMany()
                .HasForeignKey(x => x.FleteCode).HasPrincipalKey(f => f.Code)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DevolutionMotive).WithMany()
                .HasForeignKey(x => x.DevolutionMotiveCode).HasPrincipalKey(m => m.Code)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Detalles).WithOne(d => d.Guia)
                .HasForeignKey(d => d.GuiaNumber).HasPrincipalKey(g => g.Number)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.IssueDate);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.OrdenTrabajoCode);
            builder.HasIndex(x => new { x.ProgramacionType, x.ProgramacionCode });
            builder.HasIndex(x => new { x.ClienteCode, x.ObraCode });
            builder.HasIndex(x => new { x.DocumentTypeCode, x.Document1Number });
        }
    }
}
