using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Areas.ToggleAreaStatus
{
    public class ToggleAreaStatusCommandHandler : IRequestHandler<ToggleAreaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleAreaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleAreaStatusCommand request, CancellationToken cancellationToken)
        {
            var area = await _uow.Rrhh.Catalogos.Areas.Query()
                .FirstOrDefaultAsync(a => a.Code == request.Code, cancellationToken);

            if (area is null)
            {
                throw new KeyNotFoundException($"Área {request.Code} no encontrada.");
            }

            area.IsActive = !area.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return area.IsActive;
        }
    }
}