using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.GetUnidadNegocioByCode
{
    public class GetUnidadNegocioByCodeQueryHandler : IRequestHandler<GetUnidadNegocioByCodeQuery, UnidadNegocioResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetUnidadNegocioByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UnidadNegocioResponse> Handle(GetUnidadNegocioByCodeQuery request, CancellationToken cancellationToken)
        {
            var unidad = await _uow.Logistica.Catalogos.UnidadesNegocio.Query()
                .AsNoTracking()
                .Where(u => u.Code == request.Code)
                .Select(u => new UnidadNegocioResponse
                {
                    Code = u.Code,
                    Name = u.Name,
                    ParentCode = u.ParentCode,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    CreatedBy = u.CreatedBy,
                    UpdatedBy = u.UpdatedBy,
                    RowVersion = u.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (unidad is null)
            {
                throw new KeyNotFoundException($"Unidad de negocio {request.Code} no encontrada.");
            }

            return unidad;
        }
    }
}