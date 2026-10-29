using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.UpdateTipoAfiliacion
{
    public class UpdateTipoAfiliacionCommandHandler : IRequestHandler<UpdateTipoAfiliacionCommand, TipoAfiliacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoAfiliacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoAfiliacionResponse> Handle(UpdateTipoAfiliacionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.TiposAfiliacion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de afiliación {request.Code} no encontrado.");
            }

            var tipoAfiliacion = new TipoAfiliacion
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.TiposAfiliacion.Update(tipoAfiliacion);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de afiliación fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoAfiliacionResponse
            {
                Code = tipoAfiliacion.Code,
                Name = tipoAfiliacion.Name,
                IsActive = tipoAfiliacion.IsActive,
                CreatedAt = tipoAfiliacion.CreatedAt,
                UpdatedAt = tipoAfiliacion.UpdatedAt,
                RowVersion = tipoAfiliacion.RowVersion
            };
        }
    }
}