using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.UpdateUnidadNegocio
{
    public class UpdateUnidadNegocioCommandHandler : IRequestHandler<UpdateUnidadNegocioCommand, UnidadNegocioResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateUnidadNegocioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UnidadNegocioResponse> Handle(UpdateUnidadNegocioCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.UnidadesNegocio.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Unidad de negocio {request.Code} no encontrada.");
            }

            var unidad = new UnidadNegocio
            {
                Code = request.Code,
                Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim(),
                ParentCode = string.IsNullOrWhiteSpace(request.ParentCode)
                    ? null
                    : request.ParentCode.Trim().ToUpperInvariant(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.UnidadesNegocio.Update(unidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La unidad de negocio fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new UnidadNegocioResponse
            {
                Code = unidad.Code,
                Name = unidad.Name,
                ParentCode = unidad.ParentCode,
                IsActive = unidad.IsActive,
                CreatedAt = unidad.CreatedAt,
                UpdatedAt = unidad.UpdatedAt,
                CreatedBy = unidad.CreatedBy,
                UpdatedBy = unidad.UpdatedBy,
                RowVersion = unidad.RowVersion
            };
        }
    }
}