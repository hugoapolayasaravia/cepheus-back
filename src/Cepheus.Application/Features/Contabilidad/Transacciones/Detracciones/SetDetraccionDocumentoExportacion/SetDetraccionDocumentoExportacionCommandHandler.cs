using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.SetDetraccionDocumentoExportacion
{
    /// <summary>
    /// El legacy hacía cinco actualizaciones sueltas desde la ventana y sin importar el estado del lote.
    /// Ahora es una sola operación, solo en un lote Pendiente (después de emitido el archivo el depósito
    /// ya no puede cambiar).
    /// </summary>
    public class SetDetraccionDocumentoExportacionCommandHandler
        : IRequestHandler<SetDetraccionDocumentoExportacionCommand, DetraccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public SetDetraccionDocumentoExportacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DetraccionResponse> Handle(
            SetDetraccionDocumentoExportacionCommand request, CancellationToken cancellationToken)
        {
            var year = DetraccionRules.Normalize(request.Year);
            var period = DetraccionRules.Normalize(request.Period);
            var lot = DetraccionRules.Normalize(request.LotNumber);
            var proveedor = request.ProveedorCode.Trim();
            var comprobante = request.ComprobantePagoCode.Trim();
            var numero = request.NumeroDocumento.Trim();

            var detraccion = await _uow.Contabilidad.Transacciones.Detracciones.Query()
                .Include(d => d.Items)
                .ThenInclude(i => i.Documents)
                .FirstOrDefaultAsync(d => d.Year == year && d.Period == period && d.LotNumber == lot, cancellationToken);

            if (detraccion is null)
            {
                throw new KeyNotFoundException(DetraccionRules.NotFoundMessage(year, period, lot));
            }

            DetraccionRules.EnsurePendiente(detraccion, "cambiar la exportación de documentos de");

            var document = detraccion.Items
                .Where(i => i.ProveedorCode == proveedor && i.ItemNumber == request.ItemNumber)
                .SelectMany(i => i.Documents)
                .FirstOrDefault(d => d.ComprobantePagoCode == comprobante && d.NumeroDocumento.Trim() == numero);

            if (document is null)
            {
                throw new KeyNotFoundException(
                    $"El documento {comprobante} {numero} no existe en el ítem {request.ItemNumber} del proveedor {proveedor}.");
            }

            document.IncludeInExport = request.IncludeInExport;

            DetraccionRules.Recalculate(detraccion);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                _uow.ClearTracking();
                throw new InvalidOperationException(
                    "La detracción fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await DetraccionMapper.LoadAsync(_uow, year, period, lot, cancellationToken))!;
        }
    }
}
