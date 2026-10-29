using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Submodulos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Submodulos.GetSubmoduloById
{
    public class GetSubmoduloByIdQueryHandler : IRequestHandler<GetSubmoduloByIdQuery, SubmoduloResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetSubmoduloByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubmoduloResponse> Handle(GetSubmoduloByIdQuery request, CancellationToken cancellationToken)
        {
            var submodulo = await _uow.Administracion.Submodulos.Query()
                .AsNoTracking()
                .Where(s => s.Id == request.Id)
                .Select(s => new SubmoduloResponse
                {
                    Id = s.Id,
                    ModuloId = s.ModuloId,
                    Code = s.Code,
                    Name = s.Name,
                    Icon = s.Icon,
                    Tooltip = s.Tooltip,
                    DisplayOrder = s.DisplayOrder,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    CreatedBy = s.CreatedBy,
                    UpdatedBy = s.UpdatedBy,
                    RowVersion = s.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (submodulo is null)
            {
                throw new KeyNotFoundException($"Submódulo {request.Id} no encontrado.");
            }

            return submodulo;
        }
    }


}
