using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.CreatePagoProveedor
{
    /// <summary>
    /// Legacy ue_set_btn_agr_var + ue_set_btn_grb_data (paso 1): crea el pago Pendiente con la cabecera;
    /// los documentos se agregan después (SetPagoProveedorDocumentos).
    /// </summary>
    public class CreatePagoProveedorCommandHandler : IRequestHandler<CreatePagoProveedorCommand, PagoProveedorResponse>
    {
        private const int MaxAttempts = 3;

        private readonly IUnitOfWork _uow;

        public CreatePagoProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagoProveedorResponse> Handle(
            CreatePagoProveedorCommand request, CancellationToken cancellationToken)
        {
            for (var attempt = 1; ; attempt++)
            {
                var pago = new PagoProveedor
                {
                    Code = await PagoProveedorRules.NextCodeAsync(_uow, cancellationToken),
                    Estado = EstadoPagoProveedor.Pendiente,
                    Tipo = TipoPagoProveedor.Proveedores
                };

                await PagoProveedorRules.ApplyCabeceraAsync(_uow, pago, request, true, cancellationToken);

                await _uow.Contabilidad.Transacciones.PagosProveedor.AddAsync(pago, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException) when (attempt < MaxAttempts)
                {
                    // Otro usuario tomó el mismo correlativo: se vuelve a generar.
                    _uow.ClearTracking();
                    continue;
                }

                var code = pago.Code;
                _uow.ClearTracking();

                return (await PagoProveedorMapper.LoadAsync(_uow, code, cancellationToken))!;
            }
        }
    }
}
