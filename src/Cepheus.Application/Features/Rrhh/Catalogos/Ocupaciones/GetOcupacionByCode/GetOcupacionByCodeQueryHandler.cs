using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.GetOcupacionByCode
{
    public class GetOcupacionByCodeQueryHandler : IRequestHandler<GetOcupacionByCodeQuery, OcupacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetOcupacionByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OcupacionResponse> Handle(GetOcupacionByCodeQuery request, CancellationToken cancellationToken)
        {
            var ocupacion = await _uow.Rrhh.Catalogos.Ocupaciones.Query()
                .AsNoTracking()
                .Where(o => o.Code == request.Code)
                .Select(o => new OcupacionResponse
                {
                    Code = o.Code,
                    Name = o.Name,
                    IsActive = o.IsActive,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    RowVersion = o.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (ocupacion is null)
            {
                throw new KeyNotFoundException($"Ocupación {request.Code} no encontrada.");
            }

            return ocupacion;
        }
    }
}