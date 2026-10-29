using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Familias.ToggleFamiliaStatus
{
    public class ToggleFamiliaStatusCommandHandler : IRequestHandler<ToggleFamiliaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleFamiliaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleFamiliaStatusCommand request, CancellationToken cancellationToken)
        {
            // Query() (sin AsNoTracking) devuelve la entidad trackeada, igual que
            // GetByIdAsync en los catálogos con Id int — necesario acá porque
            // IRepository<T>.GetByIdAsync está tipado a int y Code es string.
            var familia = await _uow.Logistica.Catalogos.Familias.Query()
                .FirstOrDefaultAsync(f => f.Code == request.Code, cancellationToken);

            if (familia is null)
            {
                throw new KeyNotFoundException($"Familia {request.Code} no encontrada.");
            }

            familia.IsActive = !familia.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return familia.IsActive;
        }
    }
}