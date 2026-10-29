using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.ToggleAtributoConcretoStatus
{
    public class ToggleAtributoConcretoStatusCommandHandler : IRequestHandler<ToggleAtributoConcretoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleAtributoConcretoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleAtributoConcretoStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.AtributosConcreto.Query()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Atributo de concreto {request.Code} no encontrado.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
