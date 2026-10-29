using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Submodulos.Common;
using Cepheus.Domain.Administracion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Submodulos.UpdateSubmodulo
{
    public class UpdateSubmoduloCommandHandler : IRequestHandler<UpdateSubmoduloCommand, SubmoduloResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSubmoduloCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubmoduloResponse> Handle(UpdateSubmoduloCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Administracion.Submodulos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Submódulo {request.Id} no encontrado.");
            }

            var submodulo = new Submodulo
            {
                Id = request.Id,
                ModuloId = current.ModuloId, // no editable acá
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                Icon = string.IsNullOrWhiteSpace(request.Icon) ? null : request.Icon.Trim(),
                Tooltip = string.IsNullOrWhiteSpace(request.Tooltip) ? null : request.Tooltip.Trim(),
                DisplayOrder = request.DisplayOrder,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Administracion.Submodulos.Update(submodulo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El submódulo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new SubmoduloResponse
            {
                Id = submodulo.Id,
                ModuloId = submodulo.ModuloId,
                Code = submodulo.Code,
                Name = submodulo.Name,
                Icon = submodulo.Icon,
                Tooltip = submodulo.Tooltip,
                DisplayOrder = submodulo.DisplayOrder,
                IsActive = submodulo.IsActive,
                CreatedAt = submodulo.CreatedAt,
                UpdatedAt = submodulo.UpdatedAt,
                CreatedBy = submodulo.CreatedBy,
                UpdatedBy = submodulo.UpdatedBy,
                RowVersion = submodulo.RowVersion
            };
        }
    }


}
