using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.ToggleUnidadMedidaStatus
{
    public class ToggleUnidadMedidaStatusCommandHandler : IRequestHandler<ToggleUnidadMedidaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleUnidadMedidaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleUnidadMedidaStatusCommand request, CancellationToken cancellationToken)
        {
            var unidad = await _uow.Logistica.Catalogos.UnidadesMedida.Query()
                .FirstOrDefaultAsync(u => u.Code == request.Code, cancellationToken);

            if (unidad is null)
            {
                throw new KeyNotFoundException($"Unidad de medida {request.Code} no encontrada.");
            }

            unidad.IsActive = !unidad.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return unidad.IsActive;
        }
    }
}