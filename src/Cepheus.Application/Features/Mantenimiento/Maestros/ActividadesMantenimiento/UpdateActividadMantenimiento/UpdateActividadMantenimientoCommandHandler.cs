using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.UpdateActividadMantenimiento
{
    public class UpdateActividadMantenimientoCommandHandler
        : IRequestHandler<UpdateActividadMantenimientoCommand, ActividadMantenimientoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateActividadMantenimientoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ActividadMantenimientoResponse> Handle(
            UpdateActividadMantenimientoCommand request, CancellationToken cancellationToken)
        {
            var act = request.ActividadCode.Trim().ToUpperInvariant();
            var cub = request.ActividadCubiertaCode.Trim().ToUpperInvariant();

            var current = await _uow.Mantenimiento.Maestros.ActividadesMantenimiento.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ActividadCode == act && a.ActividadCubiertaCode == cub, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Registro {act}/{cub} no encontrado.");
            }

            _uow.Mantenimiento.Maestros.ActividadesMantenimiento.Update(new ActividadMantenimiento
            {
                ActividadCode = act,
                ActividadCubiertaCode = cub,
                Prioridad = request.Prioridad,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,
                RowVersion = request.RowVersion
            });

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return await _uow.Mantenimiento.Maestros.ActividadesMantenimiento.Query()
                .AsNoTracking()
                .Where(a => a.ActividadCode == act && a.ActividadCubiertaCode == cub)
                .Select(ActividadMantenimientoProjection.ToResponse)
                .FirstAsync(cancellationToken);
        }
    }
}
