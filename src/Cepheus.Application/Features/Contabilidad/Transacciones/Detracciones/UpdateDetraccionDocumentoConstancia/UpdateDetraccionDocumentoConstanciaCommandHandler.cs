using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.UpdateDetraccionDocumentoConstancia
{
    public class UpdateDetraccionDocumentoConstanciaCommandHandler
        : IRequestHandler<UpdateDetraccionDocumentoConstanciaCommand, DetraccionDocumentoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateDetraccionDocumentoConstanciaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DetraccionDocumentoResponse> Handle(
            UpdateDetraccionDocumentoConstanciaCommand request, CancellationToken cancellationToken)
        {
            var year = DetraccionRules.Normalize(request.Year);
            var period = DetraccionRules.Normalize(request.Period);
            var lot = DetraccionRules.Normalize(request.LotNumber);
            var proveedor = request.ProveedorCode.Trim();
            var comprobante = request.ComprobantePagoCode.Trim();
            var numero = request.NumeroDocumento.Trim();

            var document = await _uow.Contabilidad.Transacciones.DetraccionDocumentos.Query()
                .Include(d => d.DocumentoCompra)
                .FirstOrDefaultAsync(d => d.Year == year && d.Period == period && d.LotNumber == lot
                                          && d.ProveedorCode == proveedor && d.ItemNumber == request.ItemNumber
                                          && d.ComprobantePagoCode == comprobante
                                          && d.NumeroDocumento == numero,
                    cancellationToken);

            if (document is null)
            {
                throw new KeyNotFoundException(
                    $"El documento {comprobante} {numero} no existe en el ítem {request.ItemNumber} del proveedor {proveedor}.");
            }

            if (document.IncludeInExport)
            {
                throw new InvalidOperationException(
                    "El documento está marcado para exportar: su operación y constancia vienen del archivo de constancias " +
                    "del banco. Desmárquelo para registrarlas manualmente.");
            }

            document.DocumentoCompra.OperacionDetraccion = Clean(request.OperacionDetraccion);
            document.DocumentoCompra.ConstanciaDetraccion = Clean(request.ConstanciaDetraccion);
            document.DocumentoCompra.FechaConstanciaDetraccion = request.FechaConstanciaDetraccion?.Date;

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                _uow.ClearTracking();
                throw new InvalidOperationException(
                    "El documento fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            var response = await _uow.Contabilidad.Transacciones.DetraccionDocumentos.Query().AsNoTracking()
                .Where(d => d.Year == year && d.Period == period && d.LotNumber == lot
                            && d.ProveedorCode == proveedor && d.ItemNumber == request.ItemNumber
                            && d.ComprobantePagoCode == comprobante && d.NumeroDocumento == numero)
                .Select(DetraccionMapper.ToDocumento)
                .FirstAsync(cancellationToken);

            return DetraccionMapper.Clean(response);
        }

        private static string? Clean(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
