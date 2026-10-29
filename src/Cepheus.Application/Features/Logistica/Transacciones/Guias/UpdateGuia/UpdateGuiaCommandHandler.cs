// Cepheus.Application/Features/Logistica/Transacciones/Guias/UpdateGuia/UpdateGuiaCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.UpdateGuia
{
    /// <summary>
    /// Regla del legacy: una guía Anulada no se puede modificar. Solo se edita
    /// mientras está Pendiente.
    ///
    /// La guía se lee SIN tracking y se re-adjunta con Update(): así el
    /// RowVersion que envía el cliente es el que EF compara contra la base
    /// (con una entidad rastreada, el valor original sería el recién leído y
    /// el RowVersion del cliente no protegería contra ediciones previas).
    /// </summary>
    public class UpdateGuiaCommandHandler : IRequestHandler<UpdateGuiaCommand, GuiaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateGuiaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GuiaResponse> Handle(UpdateGuiaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var guia = await _uow.Logistica.Transacciones.Guias.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.PlantaCode == plantaCode && g.Code == code, cancellationToken);

            if (guia is null)
            {
                throw new KeyNotFoundException($"Guía {plantaCode}/{code} no encontrada.");
            }

            if (guia.Estado != EstadoGuia.Pendiente)
            {
                throw new InvalidOperationException(
                    $"La Guía está en estado '{guia.Estado}' y ya no admite modificación.");
            }

            guia.FechaEmision = request.FechaEmision.Date;
            guia.Hora = request.Hora.Trim();
            guia.ProveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();
            guia.MotivoCode = request.MotivoCode.Trim().ToUpperInvariant();
            guia.Direccion = request.Direccion.Trim();
            guia.PuntoPartida = request.PuntoPartida.Trim();
            guia.TransportistaCode = request.TransportistaCode.Trim().ToUpperInvariant();
            guia.ConductorCode = request.ConductorCode.Trim().ToUpperInvariant();
            guia.VehiculoCode = request.VehiculoCode.Trim().ToUpperInvariant();
            guia.Observaciones = request.Observaciones?.Trim() ?? string.Empty;

            guia.RowVersion = request.RowVersion;
            _uow.Logistica.Transacciones.Guias.Update(guia);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La Guía fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return await GuiaReader.GetResponseAsync(_uow, plantaCode, code, cancellationToken);
        }
    }
}
