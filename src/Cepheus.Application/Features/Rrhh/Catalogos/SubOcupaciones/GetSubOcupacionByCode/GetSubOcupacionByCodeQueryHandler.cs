using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.GetSubOcupacionByCode
{
    public class GetSubOcupacionByCodeQueryHandler : IRequestHandler<GetSubOcupacionByCodeQuery, SubOcupacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetSubOcupacionByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubOcupacionResponse> Handle(GetSubOcupacionByCodeQuery request, CancellationToken cancellationToken)
        {
            var subOcupacion = await _uow.Rrhh.Catalogos.SubOcupaciones.Query()
                .AsNoTracking()
                .Where(s => s.Code == request.Code)
                .Select(s => new SubOcupacionResponse
                {
                    Code = s.Code,
                    Name = s.Name,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    RowVersion = s.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (subOcupacion is null)
            {
                throw new KeyNotFoundException($"Sub ocupación {request.Code} no encontrada.");
            }

            return subOcupacion;
        }
    }
}