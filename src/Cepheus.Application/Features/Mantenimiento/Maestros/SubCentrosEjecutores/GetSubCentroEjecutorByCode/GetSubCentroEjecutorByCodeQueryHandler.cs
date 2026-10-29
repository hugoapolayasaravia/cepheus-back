using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.GetSubCentroEjecutorByCode
{
    public class GetSubCentroEjecutorByCodeQueryHandler
        : IRequestHandler<GetSubCentroEjecutorByCodeQuery, SubCentroEjecutorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetSubCentroEjecutorByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubCentroEjecutorResponse> Handle(
            GetSubCentroEjecutorByCodeQuery request,
            CancellationToken cancellationToken)
        {
            var subCentroEjecutor = await _uow.Mantenimiento.Maestros.SubCentrosEjecutores.Query()
                .AsNoTracking()
                .Where(s => s.Code == request.Code)
                .Select(s => new SubCentroEjecutorResponse
                {
                    Code = s.Code,
                    Name = s.Name,
                    CentroEjecutorCode = s.CentroEjecutorCode,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    RowVersion = s.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (subCentroEjecutor is null)
            {
                throw new KeyNotFoundException($"Subcentro ejecutor {request.Code} no encontrado.");
            }

            return subCentroEjecutor;
        }
    }
}
