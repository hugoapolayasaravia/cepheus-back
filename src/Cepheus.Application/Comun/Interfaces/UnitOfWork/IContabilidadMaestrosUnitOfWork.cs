using Cepheus.Domain.Contabilidad.Maestros;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IContabilidadMaestrosUnitOfWork
    {
        IRepository<CuentaContable> CuentasContables { get; }
        IRepository<Anexo> Anexos { get; }
        IRepository<CuentaBancaria> CuentasBancarias { get; }
        IRepository<PeriodoContable> PeriodosContables { get; }
        IRepository<PlantillaAsiento> PlantillasAsiento { get; }
        IRepository<PlantillaAsientoLinea> PlantillaAsientoLineas { get; }


    }
}