using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Plantas.Common;
using Cepheus.Domain.Comunes;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Plantas.CreatePlanta
{
    public class CreatePlantaCommandHandler : IRequestHandler<CreatePlantaCommand, PlantaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreatePlantaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlantaResponse> Handle(CreatePlantaCommand request, CancellationToken cancellationToken)
        {
            var planta = new Planta
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                LegalName = string.IsNullOrWhiteSpace(request.LegalName) ? null : request.LegalName.Trim(),
                Address = request.Address.Trim(),
                AddressComplement = string.IsNullOrWhiteSpace(request.AddressComplement) ? null : request.AddressComplement.Trim(),
                UbigeoCode = string.IsNullOrWhiteSpace(request.UbigeoCode) ? null : request.UbigeoCode.Trim(),
                ManagerName = string.IsNullOrWhiteSpace(request.ManagerName) ? null : request.ManagerName.Trim(),
                HasWarehouse = request.HasWarehouse,
                IsProductionPlant = request.IsProductionPlant,
                IsProject = request.IsProject,
                RequiresApprovals = request.RequiresApprovals,
                AppliesDetraction = request.AppliesDetraction,
                StatusCode = string.IsNullOrWhiteSpace(request.StatusCode) ? null : request.StatusCode.Trim(),
                IsActive = true
            };

            await _uow.Comunes.Plantas.AddAsync(planta, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(planta);
        }

        internal static PlantaResponse Map(Planta planta) => new()
        {
            Code = planta.Code,
            Name = planta.Name,
            LegalName = planta.LegalName,
            Address = planta.Address,
            AddressComplement = planta.AddressComplement,
            UbigeoCode = planta.UbigeoCode,
            ManagerName = planta.ManagerName,
            HasWarehouse = planta.HasWarehouse,
            IsProductionPlant = planta.IsProductionPlant,
            IsProject = planta.IsProject,
            RequiresApprovals = planta.RequiresApprovals,
            AppliesDetraction = planta.AppliesDetraction,
            StatusCode = planta.StatusCode,
            IsActive = planta.IsActive,
            CreatedAt = planta.CreatedAt,
            UpdatedAt = planta.UpdatedAt,
            CreatedBy = planta.CreatedBy,
            UpdatedBy = planta.UpdatedBy,
            RowVersion = planta.RowVersion
        };
    }
}
