using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Plantas.Common;
using Cepheus.Domain.Comunes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Plantas.UpdatePlanta
{
    public class UpdatePlantaCommandHandler : IRequestHandler<UpdatePlantaCommand, PlantaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePlantaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlantaResponse> Handle(UpdatePlantaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Comunes.Plantas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Planta {request.Code} no encontrada.");
            }

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

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Comunes.Plantas.Update(planta);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La planta fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new PlantaResponse
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
                RowVersion = planta.RowVersion
            };
        }
    }
}
