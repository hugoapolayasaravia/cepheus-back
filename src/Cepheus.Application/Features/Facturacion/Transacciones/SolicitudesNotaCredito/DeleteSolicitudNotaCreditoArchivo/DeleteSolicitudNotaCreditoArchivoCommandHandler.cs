using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.DeleteSolicitudNotaCreditoArchivo
{
    public class DeleteSolicitudNotaCreditoArchivoCommandHandler
        : IRequestHandler<DeleteSolicitudNotaCreditoArchivoCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public DeleteSolicitudNotaCreditoArchivoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(
            DeleteSolicitudNotaCreditoArchivoCommand request, CancellationToken cancellationToken)
        {
            var code = SolicitudNotaCreditoRules.Normalize(request.Code);
            var year = request.Year.Trim();
            var month = request.Month.Trim();
            var fileName = request.FileName.Trim();

            var status = await _uow.Facturacion.Transacciones.SolicitudesNotaCredito.Query()
                .AsNoTracking()
                .Where(s => s.Code == code && s.Year == year && s.Month == month)
                .Select(s => (EstadoSolicitudNotaCredito?)s.Status)
                .FirstOrDefaultAsync(cancellationToken);

            if (status is null)
            {
                throw new KeyNotFoundException(
                    $"Solicitud de nota de crédito {SolicitudNotaCreditoRules.FormatKey(year, month, code)} no encontrada.");
            }

            if (status != EstadoSolicitudNotaCredito.Pendiente)
            {
                throw new InvalidOperationException("Solo se pueden quitar archivos de una solicitud Pendiente.");
            }

            var archivo = await _uow.Facturacion.Transacciones.SolicitudesNotaCreditoArchivos.Query()
                .FirstOrDefaultAsync(f => f.Code == code && f.Year == year && f.Month == month && f.FileName == fileName,
                    cancellationToken);

            if (archivo is null)
            {
                throw new KeyNotFoundException($"Archivo {fileName} no encontrado en la solicitud.");
            }

            _uow.Facturacion.Transacciones.SolicitudesNotaCreditoArchivos.Remove(archivo);
            await _uow.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
