using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.AnularAsientoContable
{
    /// <summary>
    /// Anula un asiento (ver AsientoAnulacionService para las reglas).
    /// </summary>
    public class AnularAsientoContableCommandHandler
        : IRequestHandler<AnularAsientoContableCommand, AsientoContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public AnularAsientoContableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AsientoContableResponse> Handle(
            AnularAsientoContableCommand request, CancellationToken cancellationToken)
        {
            var asiento = await AsientoAnulacionService.AnularAsync(_uow, request.Codigo, cancellationToken);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El asiento, la nota de ingreso o el documento fue modificado por otro proceso. Intente nuevamente.");
            }

            return AsientoContableMapper.Map(asiento);
        }
    }
}
