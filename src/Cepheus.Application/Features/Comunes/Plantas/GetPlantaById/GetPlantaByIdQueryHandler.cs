using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Plantas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Plantas.GetPlantaById
{
    public class GetPlantaByIdQueryHandler : IRequestHandler<GetPlantaByIdQuery, PlantaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetPlantaByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlantaResponse> Handle(GetPlantaByIdQuery request, CancellationToken cancellationToken)
        {
            var planta = await _uow.Comunes.Plantas.Query()
                .AsNoTracking()
                .Where(p => p.Code == request.Code)
                .Select(p => new PlantaResponse
                {

                    Code = p.Code,
                    Name = p.Name,
                    LegalName = p.LegalName,
                    Address = p.Address,
                    AddressComplement = p.AddressComplement,
                    UbigeoCode = p.UbigeoCode,
                    ManagerName = p.ManagerName,
                    HasWarehouse = p.HasWarehouse,
                    IsProductionPlant = p.IsProductionPlant,
                    IsProject = p.IsProject,
                    RequiresApprovals = p.RequiresApprovals,
                    AppliesDetraction = p.AppliesDetraction,
                    StatusCode = p.StatusCode,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    CreatedBy = p.CreatedBy,
                    UpdatedBy = p.UpdatedBy,
                    RowVersion = p.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (planta is null)
            {
                throw new KeyNotFoundException($"Planta {request.Code} no encontrada.");
            }

            return planta;
        }
    }
}
