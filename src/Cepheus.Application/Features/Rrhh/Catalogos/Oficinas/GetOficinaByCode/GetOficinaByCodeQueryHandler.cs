using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.GetOficinaByCode
{
    public class GetOficinaByCodeQueryHandler : IRequestHandler<GetOficinaByCodeQuery, OficinaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetOficinaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OficinaResponse> Handle(GetOficinaByCodeQuery request, CancellationToken cancellationToken)
        {
            var oficina = await _uow.Rrhh.Catalogos.Oficinas.Query()
                .AsNoTracking()
                .Where(o => o.Code == request.Code)
                .Select(o => new OficinaResponse
                {
                    Code = o.Code,
                    Name = o.Name,
                    IsActive = o.IsActive,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    RowVersion = o.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (oficina is null)
            {
                throw new KeyNotFoundException($"Oficina {request.Code} no encontrada.");
            }

            return oficina;
        }
    }
}