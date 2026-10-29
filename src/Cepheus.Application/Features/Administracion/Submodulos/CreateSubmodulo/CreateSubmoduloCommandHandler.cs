using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Submodulos.Common;
using Cepheus.Domain.Administracion;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Submodulos.CreateSubmodulo
{
    public class CreateSubmoduloCommandHandler : IRequestHandler<CreateSubmoduloCommand, SubmoduloResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateSubmoduloCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubmoduloResponse> Handle(CreateSubmoduloCommand request, CancellationToken cancellationToken)
        {
            var submodulo = new Submodulo
            {
                ModuloId = request.ModuloId,
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                Icon = string.IsNullOrWhiteSpace(request.Icon) ? null : request.Icon.Trim(),
                Tooltip = string.IsNullOrWhiteSpace(request.Tooltip) ? null : request.Tooltip.Trim(),
                DisplayOrder = request.DisplayOrder,
                IsActive = true
            };

            await _uow.Administracion.Submodulos.AddAsync(submodulo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

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
