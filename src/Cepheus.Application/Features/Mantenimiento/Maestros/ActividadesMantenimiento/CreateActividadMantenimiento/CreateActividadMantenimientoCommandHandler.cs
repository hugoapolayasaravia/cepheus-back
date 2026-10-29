using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.CreateActividadMantenimiento
{
    public class CreateActividadMantenimientoCommandHandler
        : IRequestHandler<CreateActividadMantenimientoCommand, ActividadMantenimientoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateActividadMantenimientoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ActividadMantenimientoResponse> Handle(
            CreateActividadMantenimientoCommand request, CancellationToken cancellationToken)
        {
            var act = request.ActividadCode.Trim().ToUpperInvariant();
            var cub = request.ActividadCubiertaCode.Trim().ToUpperInvariant();

            await _uow.Mantenimiento.Maestros.ActividadesMantenimiento.AddAsync(new ActividadMantenimiento
            {
                ActividadCode = act,
                ActividadCubiertaCode = cub,
                Prioridad = request.Prioridad
            }, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return await _uow.Mantenimiento.Maestros.ActividadesMantenimiento.Query()
                .AsNoTracking()
                .Where(a => a.ActividadCode == act && a.ActividadCubiertaCode == cub)
                .Select(ActividadMantenimientoProjection.ToResponse)
                .FirstAsync(cancellationToken);
        }
    }
}
