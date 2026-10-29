using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.GetInspeccionByCode
{
    public class GetInspeccionByCodeQueryHandler
        : IRequestHandler<GetInspeccionByCodeQuery, InspeccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetInspeccionByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<InspeccionResponse> Handle(
            GetInspeccionByCodeQuery request,
            CancellationToken cancellationToken)
        {
            var inspeccion = await _uow.Mantenimiento.Catalogos.Inspecciones.Query()
                .AsNoTracking()
                .Where(i => i.Code == request.Code)
                .Select(i => new InspeccionResponse
                {
                    Code = i.Code,
                    Name = i.Name,
                    IsActive = i.IsActive,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    RowVersion = i.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (inspeccion is null)
            {
                throw new KeyNotFoundException($"Inspección {request.Code} no encontrada.");
            }

            return inspeccion;
        }
    }
}
