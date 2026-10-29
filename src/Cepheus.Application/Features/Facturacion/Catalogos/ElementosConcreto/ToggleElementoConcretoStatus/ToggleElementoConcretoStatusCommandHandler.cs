using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.ToggleElementoConcretoStatus
{
    public class ToggleElementoConcretoStatusCommandHandler : IRequestHandler<ToggleElementoConcretoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleElementoConcretoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleElementoConcretoStatusCommand request, CancellationToken cancellationToken)
        {
            var elementoConcreto = await _uow.Facturacion.Catalogos.ElementosConcreto.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (elementoConcreto is null)
            {
                throw new KeyNotFoundException($"Elemento de concreto {request.Code} no encontrado.");
            }

            elementoConcreto.IsActive = !elementoConcreto.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return elementoConcreto.IsActive;
        }
    }
}
