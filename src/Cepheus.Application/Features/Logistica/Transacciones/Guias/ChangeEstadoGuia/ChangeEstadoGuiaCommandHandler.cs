// Cepheus.Application/Features/Logistica/Transacciones/Guias/ChangeEstadoGuia/ChangeEstadoGuiaCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.ChangeEstadoGuia
{
    /// <summary>
    /// Única transición válida: Pendiente -> Anulado. Al anular la guía se
    /// anulan también todas sus líneas (igual que el legacy). Anular NO toca
    /// stock porque las guías no mueven stock.
    /// </summary>
    public class ChangeEstadoGuiaCommandHandler : IRequestHandler<ChangeEstadoGuiaCommand, GuiaResponse>
    {
        private static readonly Dictionary<EstadoGuia, EstadoGuia[]> ValidTransitions = new()
        {
            [EstadoGuia.Pendiente] = new[] { EstadoGuia.Anulado },
            [EstadoGuia.Anulado] = Array.Empty<EstadoGuia>()
        };

        private readonly IUnitOfWork _uow;

        public ChangeEstadoGuiaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GuiaResponse> Handle(ChangeEstadoGuiaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            if (!System.Enum.TryParse<EstadoGuia>(request.NuevoEstado, true, out var nuevoEstado)
                || !System.Enum.IsDefined(typeof(EstadoGuia), nuevoEstado))
            {
                throw new InvalidOperationException($"Estado '{request.NuevoEstado}' no es válido.");
            }

            var guia = await _uow.Logistica.Transacciones.Guias.Query()
                .Include(g => g.Detalles)
                .FirstOrDefaultAsync(g => g.PlantaCode == plantaCode && g.Code == code, cancellationToken);

            if (guia is null)
            {
                throw new KeyNotFoundException($"Guía {plantaCode}/{code} no encontrada.");
            }

            if (guia.Estado == nuevoEstado)
            {
                throw new InvalidOperationException($"La Guía ya se encuentra en estado '{guia.Estado}'.");
            }

            if (!ValidTransitions[guia.Estado].Contains(nuevoEstado))
            {
                throw new InvalidOperationException(
                    $"No se puede pasar de '{guia.Estado}' a '{nuevoEstado}'. " +
                    $"Transiciones válidas desde '{guia.Estado}': " +
                    $"{(ValidTransitions[guia.Estado].Length == 0 ? "ninguna" : string.Join(", ", ValidTransitions[guia.Estado]))}.");
            }

            guia.Estado = nuevoEstado;

            if (nuevoEstado == EstadoGuia.Anulado)
            {
                foreach (var detalle in guia.Detalles)
                {
                    detalle.Estado = EstadoGuiaDetalle.Anulado;
                }
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return await GuiaReader.GetResponseAsync(_uow, plantaCode, code, cancellationToken);
        }
    }
}
