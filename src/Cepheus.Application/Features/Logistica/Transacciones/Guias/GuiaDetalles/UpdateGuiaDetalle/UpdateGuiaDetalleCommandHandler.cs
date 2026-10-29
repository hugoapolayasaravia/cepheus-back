// Cepheus.Application/Features/Logistica/Transacciones/Guias/GuiaDetalles/UpdateGuiaDetalle/UpdateGuiaDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.GuiaDetalles.UpdateGuiaDetalle
{
    /// <summary>
    /// Igual que UpdateGuiaCommandHandler: la línea se lee sin tracking y se
    /// re-adjunta con Update() para que el RowVersion del cliente sea el que
    /// se compara contra la base.
    /// </summary>
    public class UpdateGuiaDetalleCommandHandler : IRequestHandler<UpdateGuiaDetalleCommand, GuiaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateGuiaDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GuiaResponse> Handle(UpdateGuiaDetalleCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var guiaCode = request.GuiaCode.Trim().ToUpperInvariant();
            var articuloCode = request.ArticuloCode.Trim().ToUpperInvariant();

            var estadoGuia = await _uow.Logistica.Transacciones.Guias.Query()
                .AsNoTracking()
                .Where(g => g.PlantaCode == plantaCode && g.Code == guiaCode)
                .Select(g => (EstadoGuia?)g.Estado)
                .FirstOrDefaultAsync(cancellationToken);

            if (estadoGuia is null)
            {
                throw new KeyNotFoundException($"Guía {plantaCode}/{guiaCode} no encontrada.");
            }

            if (estadoGuia != EstadoGuia.Pendiente)
            {
                throw new InvalidOperationException(
                    $"La Guía está en estado '{estadoGuia}' y ya no admite editar líneas.");
            }

            var linea = await _uow.Logistica.Transacciones.GuiaDetalles.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.PlantaCode == plantaCode
                                       && d.GuiaCode == guiaCode
                                       && d.ArticuloCode == articuloCode, cancellationToken);

            if (linea is null)
            {
                throw new KeyNotFoundException(
                    $"El artículo {articuloCode} no está en la guía {plantaCode}/{guiaCode}.");
            }

            linea.Cantidad = request.Cantidad;
            linea.IsVerified = request.IsVerified;
            linea.RowVersion = request.RowVersion;

            _uow.Logistica.Transacciones.GuiaDetalles.Update(linea);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La línea fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return await GuiaReader.GetResponseAsync(_uow, plantaCode, guiaCode, cancellationToken);
        }
    }
}
