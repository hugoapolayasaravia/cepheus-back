// Cepheus.Application/Features/Logistica/Transacciones/Guias/Common/GuiaReader.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.Common
{
    /// <summary>
    /// Lee la guía completa desde la base (sin tracking) y la mapea a
    /// GuiaResponse. Lo usan todos los commands para devolver el estado final
    /// ya persistido, con nombres de maestros y RowVersion actualizado.
    /// </summary>
    internal static class GuiaReader
    {
        public static async Task<GuiaResponse> GetResponseAsync(
            IUnitOfWork uow,
            string plantaCode,
            string code,
            CancellationToken cancellationToken)
        {
            var guia = await uow.Logistica.Transacciones.Guias.Query()
                .AsNoTracking()
                .WithFullIncludes()
                .FirstOrDefaultAsync(g => g.PlantaCode == plantaCode && g.Code == code, cancellationToken);

            if (guia is null)
            {
                throw new KeyNotFoundException($"Guía {plantaCode}/{code} no encontrada.");
            }

            return GuiaMapper.ToResponse(guia);
        }
    }
}
