using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.Common;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.UpdateOTRMaterial
{
    public class UpdateOTRMaterialCommandHandler : IRequestHandler<UpdateOTRMaterialCommand, OTRMaterialResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateOTRMaterialCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OTRMaterialResponse> Handle(UpdateOTRMaterialCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var ordenCode = request.OrdenTrabajoCode.Trim().ToUpperInvariant();
            var articuloCode = request.ArticuloCode.Trim().ToUpperInvariant();

            var current = await _uow.Mantenimiento.Transacciones.OTRMateriales.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(o =>
                    o.PlantaCode == plantaCode &&
                    o.OrdenTrabajoCode == ordenCode &&
                    o.FechaProceso == request.FechaProceso &&
                    o.ArticuloCode == articuloCode,
                    cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException(
                    $"Registro de material {plantaCode}/{ordenCode}/{articuloCode} en {request.FechaProceso:yyyy-MM-dd} no encontrado.");
            }

            var otrMaterial = new OTRMaterial
            {
                PlantaCode = plantaCode,
                OrdenTrabajoCode = ordenCode,
                FechaProceso = request.FechaProceso,
                ArticuloCode = articuloCode,

                Cantidad = request.Cantidad,
                CostoUnitario = request.CostoUnitario,
                CostoTotal = request.CostoTotal,
                EstadoCode = string.IsNullOrWhiteSpace(request.EstadoCode) ? null : request.EstadoCode.Trim().ToUpperInvariant(),

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Transacciones.OTRMateriales.Update(otrMaterial);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateOTRMaterial.CreateOTRMaterialCommandHandler.Map(otrMaterial);
        }
    }
}
