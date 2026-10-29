// Cepheus.Application/Features/Logistica/Transacciones/Guias/GuiaDetalles/DeleteGuiaDetalle/DeleteGuiaDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.GuiaDetalles.DeleteGuiaDetalle
{
    /// <summary>
    /// Borrado real de la línea (mismo criterio que PedidoDetalle). Exige que
    /// la guía conserve al menos una línea y renumera ItemNumber de las
    /// restantes (1..n), equivalente al legacy Logi_sp_Actualiza_Item_Gui.
    /// </summary>
    public class DeleteGuiaDetalleCommandHandler : IRequestHandler<DeleteGuiaDetalleCommand, GuiaResponse>
    {
        private readonly IUnitOfWork _uow;

        public DeleteGuiaDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GuiaResponse> Handle(DeleteGuiaDetalleCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var guiaCode = request.GuiaCode.Trim().ToUpperInvariant();
            var articuloCode = request.ArticuloCode.Trim().ToUpperInvariant();

            var guia = await _uow.Logistica.Transacciones.Guias.Query()
                .Include(g => g.Detalles)
                .FirstOrDefaultAsync(g => g.PlantaCode == plantaCode && g.Code == guiaCode, cancellationToken);

            if (guia is null)
            {
                throw new KeyNotFoundException($"Guía {plantaCode}/{guiaCode} no encontrada.");
            }

            if (guia.Estado != EstadoGuia.Pendiente)
            {
                throw new InvalidOperationException(
                    $"La Guía está en estado '{guia.Estado}' y ya no admite eliminar líneas.");
            }

            var linea = guia.Detalles.FirstOrDefault(d => d.ArticuloCode == articuloCode);
            if (linea is null)
            {
                throw new KeyNotFoundException(
                    $"El artículo {articuloCode} no está en la guía {plantaCode}/{guiaCode}.");
            }

            if (guia.Detalles.Count == 1)
            {
                throw new InvalidOperationException("La Guía debe conservar al menos una línea de detalle.");
            }

            guia.Detalles.Remove(linea);
            _uow.Logistica.Transacciones.GuiaDetalles.Remove(linea);

            var item = 1;
            foreach (var restante in guia.Detalles.OrderBy(d => d.ItemNumber))
            {
                restante.ItemNumber = item++;
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return await GuiaReader.GetResponseAsync(_uow, plantaCode, guiaCode, cancellationToken);
        }
    }
}
