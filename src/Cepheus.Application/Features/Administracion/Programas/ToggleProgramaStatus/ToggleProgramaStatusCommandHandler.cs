using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Programas.ToggleProgramaStatus
{
    public class ToggleProgramaStatusCommandHandler : IRequestHandler<ToggleProgramaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleProgramaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleProgramaStatusCommand request, CancellationToken cancellationToken)
        {
            var programa = await _uow.Administracion.Programas.GetByIdAsync(request.Id, cancellationToken);

            if (programa is null)
            {
                throw new KeyNotFoundException($"Programa {request.Id} no encontrado.");
            }

            programa.IsActive = !programa.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return programa.IsActive;
        }
    }

}
