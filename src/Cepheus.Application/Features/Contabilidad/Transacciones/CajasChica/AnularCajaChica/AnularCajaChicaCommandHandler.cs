using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.AnularCajaChica
{
    /// <summary>
    /// Anula una caja chica Pendiente y sin asientos: montos en cero y detalle eliminado
    /// (legacy ue_set_btn_eli_bor). Una caja Procesada o Cancelada debe reabrirse antes.
    /// </summary>
    public class AnularCajaChicaCommandHandler : IRequestHandler<AnularCajaChicaCommand, CajaChicaResponse>
    {
        private readonly IUnitOfWork _uow;

        public AnularCajaChicaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CajaChicaResponse> Handle(AnularCajaChicaCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var caja = await _uow.Contabilidad.Transacciones.CajasChica.Query()
                .Include(c => c.Details)
                .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);

            if (caja is null)
            {
                throw new KeyNotFoundException($"Caja chica {code} no encontrada.");
            }

            if (caja.Status == EstadoCajaChica.Anulado)
            {
                throw new InvalidOperationException("La caja chica ya está anulada.");
            }

            if (caja.Status != EstadoCajaChica.Pendiente || CajaChicaRules.HasAsientos(caja))
            {
                throw new InvalidOperationException(
                    "Solo se puede anular una caja chica Pendiente y sin asientos. Reabra la caja antes de anularla.");
            }

            foreach (var detail in caja.Details.ToList())
            {
                _uow.Contabilidad.Transacciones.CajasChicaDetalle.Remove(detail);
            }

            caja.MontoSol = 0m;
            caja.TotalSol = 0m;
            caja.IsPrinted = false;
            caja.Status = EstadoCajaChica.Anulado;

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La caja chica fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await CajaChicaMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
