using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.UpdateInspeccion
{
    public class UpdateInspeccionCommandHandler : IRequestHandler<UpdateInspeccionCommand, InspeccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateInspeccionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<InspeccionResponse> Handle(UpdateInspeccionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Mantenimiento.Catalogos.Inspecciones.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Inspección {request.Code} no encontrada.");
            }

            var inspeccion = new Inspeccion
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Catalogos.Inspecciones.Update(inspeccion);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La inspección fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new InspeccionResponse
            {
                Code = inspeccion.Code,
                Name = inspeccion.Name,
                IsActive = inspeccion.IsActive,
                CreatedAt = inspeccion.CreatedAt,
                UpdatedAt = inspeccion.UpdatedAt,
                RowVersion = inspeccion.RowVersion
            };
        }
    }
}
