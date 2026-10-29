using Cepheus.Domain.Rrhh.Maestros;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{
    // Recusos Humanos - Maestros
    public DbSet<Trabajador> Trabajadores => Set<Trabajador>();

    public DbSet<TrabajadorDocumento> TrabajadorDocumentos => Set<TrabajadorDocumento>();

    public DbSet<TrabajadorDomicilio> TrabajadorDomicilios => Set<TrabajadorDomicilio>();
    public DbSet<TrabajadorContacto> TrabajadorContactos => Set<TrabajadorContacto>();
    public DbSet<TrabajadorLaboral> TrabajadorLaborals => Set<TrabajadorLaboral>();
    public DbSet<TrabajadorContrato> TrabajadorContratos => Set<TrabajadorContrato>();
    public DbSet<TrabajadorFormacion> TrabajadorFormacions => Set<TrabajadorFormacion>();
    public DbSet<TrabajadorRemuneracion> TrabajadorRemuneracions => Set<TrabajadorRemuneracion>();
    public DbSet<TrabajadorCuentaBancaria> TrabajadorCuentaBancarias => Set<TrabajadorCuentaBancaria>();
    public DbSet<TrabajadorPension> TrabajadorPensions => Set<TrabajadorPension>();
    public DbSet<TrabajadorSeguro> TrabajadorSeguros => Set<TrabajadorSeguro>();
    public DbSet<TrabajadorJornada> TrabajadorJornadas => Set<TrabajadorJornada>();
    public DbSet<TrabajadorBeneficio> TrabajadorBeneficios => Set<TrabajadorBeneficio>();
    public DbSet<TrabajadorDependiente> TrabajadorDependientes => Set<TrabajadorDependiente>();
    public DbSet<TrabajadorSalud> TrabajadorSaluds => Set<TrabajadorSalud>();
    public DbSet<TrabajadorSindicato> TrabajadorSindicatos => Set<TrabajadorSindicato>();
    public DbSet<TrabajadorAntecedente> TrabajadorAntecedentes => Set<TrabajadorAntecedente>();
    public DbSet<TrabajadorVacacion> TrabajadorVacacions => Set<TrabajadorVacacion>();
    public DbSet<TrabajadorFiscal> TrabajadorFiscals => Set<TrabajadorFiscal>();
    public DbSet<TrabajadorContable> TrabajadorContables => Set<TrabajadorContable>();

}