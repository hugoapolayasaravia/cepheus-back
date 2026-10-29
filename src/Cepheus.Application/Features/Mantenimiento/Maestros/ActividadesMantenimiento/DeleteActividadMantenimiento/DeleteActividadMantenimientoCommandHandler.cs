using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.DeleteActividadMantenimiento
{
    public class DeleteActividadMantenimientoCommandHandler : IRequestHandler<DeleteActividadMantenimientoCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteActividadMantenimientoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteActividadMantenimientoCommand request, CancellationToken cancellationToken)
        {
            var act = request.ActividadCode.Trim().ToUpperInvariant();
            var cub = request.ActividadCubiertaCode.Trim().ToUpperInvariant();

            var entity = await _uow.Mantenimiento.Maestros.ActividadesMantenimiento.Query()
                .FirstOrDefaultAsync(a => a.ActividadCode == act && a.ActividadCubiertaCode == cub, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Registro {act}/{cub} no encontrado.");
            }

            _uow.Mantenimiento.Maestros.ActividadesMantenimiento.Remove(entity);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
