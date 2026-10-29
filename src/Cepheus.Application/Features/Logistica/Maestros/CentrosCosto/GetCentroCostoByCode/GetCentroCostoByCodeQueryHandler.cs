using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.GetCentroCostoByCode
{
    public class GetCentroCostoByCodeQueryHandler : IRequestHandler<GetCentroCostoByCodeQuery, CentroCostoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetCentroCostoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CentroCostoResponse> Handle(GetCentroCostoByCodeQuery request, CancellationToken cancellationToken)
        {
            var centro = await _uow.Logistica.Maestros.CentrosCosto.Query()
                .AsNoTracking()
                .Where(c => c.Code == request.Code)
                .Select(c => new CentroCostoResponse
                {
                    Code = c.Code,
                    Name = c.Name,
                    PlantaCode = c.PlantaCode,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    RowVersion = c.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (centro is null)
            {
                throw new KeyNotFoundException($"Centro de costo {request.Code} no encontrado.");
            }

            return centro;
        }
    }
}
