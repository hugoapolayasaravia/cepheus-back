using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.ToggleTecnicoStatus
{
    public class ToggleTecnicoStatusCommandHandler : IRequestHandler<ToggleTecnicoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTecnicoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTecnicoStatusCommand request, CancellationToken cancellationToken)
        {
            var tecnico = await _uow.Facturacion.Maestros.Tecnicos.Query()
                .FirstOrDefaultAsync(t => t.TrabajadorCode == request.TrabajadorCode, cancellationToken);

            if (tecnico is null)
            {
                throw new KeyNotFoundException($"Técnico {request.TrabajadorCode} no encontrado.");
            }

            tecnico.IsActive = !tecnico.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tecnico.IsActive;
        }
    }
}
