using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.DeleteOTRMaterial
{
    public class DeleteOTRMaterialCommandHandler : IRequestHandler<DeleteOTRMaterialCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteOTRMaterialCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteOTRMaterialCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var ordenCode = request.OrdenTrabajoCode.Trim().ToUpperInvariant();
            var articuloCode = request.ArticuloCode.Trim().ToUpperInvariant();

            var otrMaterial = await _uow.Mantenimiento.Transacciones.OTRMateriales.Query()
                .FirstOrDefaultAsync(o =>
                    o.PlantaCode == plantaCode &&
                    o.OrdenTrabajoCode == ordenCode &&
                    o.FechaProceso == request.FechaProceso &&
                    o.ArticuloCode == articuloCode,
                    cancellationToken);

            if (otrMaterial is null)
            {
                throw new KeyNotFoundException(
                    $"Registro de material {plantaCode}/{ordenCode}/{articuloCode} en {request.FechaProceso:yyyy-MM-dd} no encontrado.");
            }

            _uow.Mantenimiento.Transacciones.OTRMateriales.Remove(otrMaterial);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
