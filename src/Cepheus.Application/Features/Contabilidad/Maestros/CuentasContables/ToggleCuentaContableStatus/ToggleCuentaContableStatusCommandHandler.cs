using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.ToggleCuentaContableStatus
{
    public class ToggleCuentaContableStatusCommandHandler : IRequestHandler<ToggleCuentaContableStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleCuentaContableStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleCuentaContableStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Maestros.CuentasContables.Query()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Cuenta contable {request.Code} no encontrado.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
