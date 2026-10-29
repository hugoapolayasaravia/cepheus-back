using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.UpdateTipoZona
{
    public class UpdateTipoZonaCommandHandler : IRequestHandler<UpdateTipoZonaCommand, TipoZonaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoZonaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoZonaResponse> Handle(UpdateTipoZonaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.TiposZona.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de zona {request.Code} no encontrado.");
            }

            var tipoZona = new TipoZona
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.TiposZona.Update(tipoZona);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de zona fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoZonaResponse
            {
                Code = tipoZona.Code,
                Name = tipoZona.Name,
                IsActive = tipoZona.IsActive,
                CreatedAt = tipoZona.CreatedAt,
                UpdatedAt = tipoZona.UpdatedAt,
                RowVersion = tipoZona.RowVersion
            };
        }
    }
}