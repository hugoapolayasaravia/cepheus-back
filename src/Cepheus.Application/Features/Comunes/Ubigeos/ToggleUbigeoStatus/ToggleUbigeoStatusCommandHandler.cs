using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Ubigeos.ToggleUbigeoStatus
{
    public class ToggleUbigeoStatusCommandHandler : IRequestHandler<ToggleUbigeoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleUbigeoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleUbigeoStatusCommand request, CancellationToken cancellationToken)
        {
            var ubigeo = await _uow.Comunes.Ubigeos.GetByCodeAsync(request.Code, cancellationToken);

            if (ubigeo is null)
            {
                throw new KeyNotFoundException($"Ubigeo {request.Code} no encontrado.");
            }

            ubigeo.IsActive = !ubigeo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return ubigeo.IsActive;
        }
    }
}
