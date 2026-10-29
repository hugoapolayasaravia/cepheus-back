using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.ImprimirCajaChica
{
    public class ImprimirCajaChicaCommandHandler : IRequestHandler<ImprimirCajaChicaCommand, CajaChicaResponse>
    {
        private readonly IUnitOfWork _uow;

        public ImprimirCajaChicaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CajaChicaResponse> Handle(ImprimirCajaChicaCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var caja = await _uow.Contabilidad.Transacciones.CajasChica.Query()
                .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);

            if (caja is null)
            {
                throw new KeyNotFoundException($"Caja chica {code} no encontrada.");
            }

            if (caja.Status == EstadoCajaChica.Anulado)
            {
                throw new InvalidOperationException("La caja chica está anulada.");
            }

            caja.IsPrinted = request.Printed;

            await _uow.SaveChangesAsync(cancellationToken);
            _uow.ClearTracking();

            return (await CajaChicaMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
