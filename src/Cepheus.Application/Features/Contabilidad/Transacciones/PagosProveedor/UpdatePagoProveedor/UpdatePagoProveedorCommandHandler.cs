using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.UpdatePagoProveedor
{
    /// <summary>
    /// Legacy ue_set_btn_mod_dw: con asiento generado no se edita; con documentos ya cargados no se
    /// cambia el proveedor ni la fecha de emisión.
    /// </summary>
    public class UpdatePagoProveedorCommandHandler : IRequestHandler<UpdatePagoProveedorCommand, PagoProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePagoProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagoProveedorResponse> Handle(
            UpdatePagoProveedorCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var pago = await _uow.Contabilidad.Transacciones.PagosProveedor.Query()
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);

            if (pago is null)
            {
                throw new KeyNotFoundException($"Pago {code} no encontrado.");
            }

            PagoProveedorRules.EnsureEditable(pago);

            await PagoProveedorRules.ApplyCabeceraAsync(
                _uow, pago, request, pago.Detalles.Count == 0, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);
            _uow.ClearTracking();

            return (await PagoProveedorMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
