using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.AddSolicitudNotaCreditoArchivo
{
    public class AddSolicitudNotaCreditoArchivoCommandHandler
        : IRequestHandler<AddSolicitudNotaCreditoArchivoCommand, SolicitudNotaCreditoArchivoResponse>
    {
        private readonly IUnitOfWork _uow;

        public AddSolicitudNotaCreditoArchivoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SolicitudNotaCreditoArchivoResponse> Handle(
            AddSolicitudNotaCreditoArchivoCommand request, CancellationToken cancellationToken)
        {
            var code = SolicitudNotaCreditoRules.Normalize(request.Code);
            var year = request.Year.Trim();
            var month = request.Month.Trim();
            var fileName = request.FileName.Trim();

            var solicitud = await _uow.Facturacion.Transacciones.SolicitudesNotaCredito.Query()
                .AsNoTracking()
                .Where(s => s.Code == code && s.Year == year && s.Month == month)
                .Select(s => new { s.Status })
                .FirstOrDefaultAsync(cancellationToken);

            if (solicitud is null)
            {
                throw new KeyNotFoundException(
                    $"Solicitud de nota de crédito {SolicitudNotaCreditoRules.FormatKey(year, month, code)} no encontrada.");
            }

            if (solicitud.Status != EstadoSolicitudNotaCredito.Pendiente)
            {
                throw new InvalidOperationException("Solo se pueden adjuntar archivos a una solicitud Pendiente.");
            }

            var exists = await _uow.Facturacion.Transacciones.SolicitudesNotaCreditoArchivos.Query()
                .AnyAsync(f => f.Code == code && f.Year == year && f.Month == month && f.FileName == fileName,
                    cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException($"Ya existe un archivo con el nombre {fileName} en la solicitud.");
            }

            var archivo = new SolicitudNotaCreditoArchivo
            {
                Code = code,
                Year = year,
                Month = month,
                FileName = fileName,
                FilePath = request.FilePath.Trim()
            };

            await _uow.Facturacion.Transacciones.SolicitudesNotaCreditoArchivos.AddAsync(archivo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return new SolicitudNotaCreditoArchivoResponse { FileName = archivo.FileName, FilePath = archivo.FilePath };
        }
    }
}
