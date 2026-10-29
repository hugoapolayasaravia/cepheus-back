using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.ToggleTipoCuentaContableStatus
{
    public class ToggleTipoCuentaContableStatusCommandHandler : IRequestHandler<ToggleTipoCuentaContableStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoCuentaContableStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoCuentaContableStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Catalogos.TiposCuentaContable.Query()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Tipo de cuenta contable {request.Code} no encontrado.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
