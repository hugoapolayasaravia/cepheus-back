using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.GetEstadoCivilByCode
{
    public class GetEstadoCivilByCodeQueryHandler : IRequestHandler<GetEstadoCivilByCodeQuery, EstadoCivilResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetEstadoCivilByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EstadoCivilResponse> Handle(GetEstadoCivilByCodeQuery request, CancellationToken cancellationToken)
        {
            var estadoCivil = await _uow.Rrhh.Catalogos.EstadosCiviles.Query()
                .AsNoTracking()
                .Where(e => e.Code == request.Code)
                .Select(e => new EstadoCivilResponse
                {
                    Code = e.Code,
                    Name = e.Name,
                    IsActive = e.IsActive,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    RowVersion = e.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (estadoCivil is null)
            {
                throw new KeyNotFoundException($"Estado civil {request.Code} no encontrado.");
            }

            return estadoCivil;
        }
    }
}