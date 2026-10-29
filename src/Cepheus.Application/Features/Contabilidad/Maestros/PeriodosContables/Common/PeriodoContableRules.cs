using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Enum;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common
{
    /// <summary>
    /// Control de período contable (reemplaza la comparación con gs_cierre del legacy).
    /// Lo usan los procesos que generan o anulan asientos.
    /// </summary>
    public static class PeriodoContableRules
    {
        public static string BuildCode(string anio, string mes) => anio.Trim() + mes.Trim();

        /// <summary>El período debe existir y estar Abierto; de lo contrario lanza InvalidOperationException.</summary>
        public static async Task EnsureOpenAsync(
            IUnitOfWork uow, string anio, string mes, CancellationToken cancellationToken)
        {
            var code = BuildCode(anio, mes);

            var periodo = await uow.Contabilidad.Maestros.PeriodosContables.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);

            if (periodo is null)
            {
                throw new InvalidOperationException(
                    $"El período contable {anio.Trim()}-{mes.Trim()} no está registrado. Regístrelo antes de continuar.");
            }

            if (periodo.Status != EstadoPeriodoContable.Abierto)
            {
                throw new InvalidOperationException(
                    $"El período contable {anio.Trim()}-{mes.Trim()} está cerrado. Reabra el período para continuar.");
            }
        }
    }
}
