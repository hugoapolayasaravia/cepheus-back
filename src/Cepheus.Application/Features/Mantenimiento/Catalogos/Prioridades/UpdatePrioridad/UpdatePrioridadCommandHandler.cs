using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.UpdatePrioridad
{
    public class UpdatePrioridadCommandHandler : IRequestHandler<UpdatePrioridadCommand, PrioridadResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePrioridadCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PrioridadResponse> Handle(UpdatePrioridadCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Mantenimiento.Catalogos.Prioridades.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Prioridad {request.Code} no encontrada.");
            }

            var prioridad = new Prioridad
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Catalogos.Prioridades.Update(prioridad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La prioridad fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new PrioridadResponse
            {
                Code = prioridad.Code,
                Name = prioridad.Name,
                IsActive = prioridad.IsActive,
                CreatedAt = prioridad.CreatedAt,
                UpdatedAt = prioridad.UpdatedAt,
                RowVersion = prioridad.RowVersion
            };
        }
    }
}
