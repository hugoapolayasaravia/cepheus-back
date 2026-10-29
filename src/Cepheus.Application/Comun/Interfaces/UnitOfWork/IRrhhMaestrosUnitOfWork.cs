using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IRrhhMaestrosUnitOfWork
    {

        IRepository<Trabajador> Trabajadores { get; }
        IRepository<TrabajadorDocumento> TrabajadorDocumentos { get; }
        IRepository<TrabajadorDomicilio> TrabajadorDomicilios { get; }
        IRepository<TrabajadorContacto> TrabajadorContactos { get; }

        IRepository<TrabajadorLaboral> TrabajadorLaborals { get; }
        IRepository<TrabajadorContrato> TrabajadorContratos { get; }
        IRepository<TrabajadorFormacion> TrabajadorFormacions { get; }
        IRepository<TrabajadorRemuneracion> TrabajadorRemuneracions { get; }
        IRepository<TrabajadorCuentaBancaria> TrabajadorCuentaBancarias { get; }
        IRepository<TrabajadorPension> TrabajadorPensions { get; }
        IRepository<TrabajadorSeguro> TrabajadorSeguros { get; }
        IRepository<TrabajadorJornada> TrabajadorJornadas { get; }
        IRepository<TrabajadorBeneficio> TrabajadorBeneficios { get; }
        IRepository<TrabajadorDependiente> TrabajadorDependientes { get; }
        IRepository<TrabajadorSalud> TrabajadorSaluds { get; }
        IRepository<TrabajadorSindicato> TrabajadorSindicatos { get; }
        IRepository<TrabajadorAntecedente> TrabajadorAntecedentes { get; }
        IRepository<TrabajadorVacacion> TrabajadorVacacions { get; }
        IRepository<TrabajadorFiscal> TrabajadorFiscals { get; }
        IRepository<TrabajadorContable> TrabajadorContables { get; }

    }
}