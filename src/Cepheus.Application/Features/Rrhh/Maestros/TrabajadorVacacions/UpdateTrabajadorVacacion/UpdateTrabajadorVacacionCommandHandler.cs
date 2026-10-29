using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.UpdateTrabajadorVacacion
{
    public class UpdateTrabajadorVacacionCommandHandler : IRequestHandler<UpdateTrabajadorVacacionCommand, TrabajadorVacacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorVacacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorVacacionResponse> Handle(UpdateTrabajadorVacacionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorVacacions.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorVacacion {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorVacacion
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                FechaVacaciones = request.FechaVacaciones,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorVacacions.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorVacacion.CreateTrabajadorVacacionCommandHandler.Map(entidad);
        }
    }
}
