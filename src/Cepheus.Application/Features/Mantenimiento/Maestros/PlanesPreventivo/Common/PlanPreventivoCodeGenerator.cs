using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common
{
    /// <summary>
    /// Código del plan preventivo: 'MP' + correlativo de 4 dígitos por planta (ej. MP0001).
    /// Legacy (Logi_sp_Actualiza_ManPreventivo): MAX(RIGHT(Codigo_Mpv, 4)) + 1 por planta.
    /// </summary>
    public static class PlanPreventivoCodeGenerator
    {
        private const string Prefix = "MP";
        private const int NumberLength = 4;
        private const int MaxNumber = 9999;

        public static async Task<string> NextCodeAsync(IUnitOfWork uow, string plantaCode, CancellationToken cancellationToken)
        {
            var codes = await uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query()
                .AsNoTracking()
                .Where(x => x.PlantaCode == plantaCode)
                .Select(x => x.Code)
                .ToListAsync(cancellationToken);

            var max = codes
                .Select(c => c.Length >= NumberLength && int.TryParse(c[^NumberLength..], out var n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();

            var next = max + 1;
            if (next > MaxNumber)
            {
                throw new InvalidOperationException(
                    $"Se agotó el correlativo de planes preventivos de la planta {plantaCode} (máximo {Prefix}{MaxNumber}).");
            }

            return Prefix + next.ToString().PadLeft(NumberLength, '0');
        }
    }
}
