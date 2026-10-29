using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.GetCentroEjecutorByCode
{
    public class GetCentroEjecutorByCodeQueryHandler
        : IRequestHandler<GetCentroEjecutorByCodeQuery, CentroEjecutorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetCentroEjecutorByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CentroEjecutorResponse> Handle(
            GetCentroEjecutorByCodeQuery request,
            CancellationToken cancellationToken)
        {
            var centroEjecutor = await _uow.Mantenimiento.Maestros.CentrosEjecutores.Query()
                .AsNoTracking()
                .Where(c => c.Code == request.Code)
                .Select(c => new CentroEjecutorResponse
                {
                    Code = c.Code,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    RowVersion = c.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (centroEjecutor is null)
            {
                throw new KeyNotFoundException($"Centro ejecutor {request.Code} no encontrado.");
            }

            return centroEjecutor;
        }
    }
}
