using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.ToggleCuentaBancariaStatus
{
    public class ToggleCuentaBancariaStatusCommandHandler : IRequestHandler<ToggleCuentaBancariaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleCuentaBancariaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleCuentaBancariaStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Maestros.CuentasBancarias.Query()
                .FirstOrDefaultAsync(x => x.BankCode == request.BankCode && x.AccountNumber == request.AccountNumber, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Cuenta bancaria {request.BankCode}/{request.AccountNumber} no encontrado.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
