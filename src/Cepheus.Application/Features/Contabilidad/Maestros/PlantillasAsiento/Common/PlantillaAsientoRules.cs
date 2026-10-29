using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Enum;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common
{
    public static class PlantillaAsientoRules
    {
        /// <summary>Solo puede haber una plantilla ACTIVA por proceso.</summary>
        public static async Task EnsureSingleActiveAsync(
            IUnitOfWork uow, ProcesoAsiento process, string? excludeCode, CancellationToken cancellationToken)
        {
            var other = await uow.Contabilidad.Maestros.PlantillasAsiento.Query()
                .AsNoTracking()
                .Where(p => p.Process == process && p.IsActive && (excludeCode == null || p.Code != excludeCode))
                .Select(p => p.Code)
                .FirstOrDefaultAsync(cancellationToken);

            if (other is not null)
            {
                throw new InvalidOperationException(
                    $"Ya existe una plantilla activa ({other}) para el proceso {process}. Desactívela antes de activar otra.");
            }
        }
    }
}
