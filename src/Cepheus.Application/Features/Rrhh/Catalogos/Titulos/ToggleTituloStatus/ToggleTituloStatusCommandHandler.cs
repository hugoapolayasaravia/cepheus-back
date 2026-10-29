using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Titulos.ToggleTituloStatus
{
    public class ToggleTituloStatusCommandHandler : IRequestHandler<ToggleTituloStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTituloStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTituloStatusCommand request, CancellationToken cancellationToken)
        {
            var titulo = await _uow.Rrhh.Catalogos.Titulos.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (titulo is null)
            {
                throw new KeyNotFoundException($"Título {request.Code} no encontrado.");
            }

            titulo.IsActive = !titulo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return titulo.IsActive;
        }
    }
}