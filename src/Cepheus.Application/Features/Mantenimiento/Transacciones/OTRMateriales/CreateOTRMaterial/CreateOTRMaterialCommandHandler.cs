using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.Common;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.CreateOTRMaterial
{
    public class CreateOTRMaterialCommandHandler : IRequestHandler<CreateOTRMaterialCommand, OTRMaterialResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateOTRMaterialCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OTRMaterialResponse> Handle(CreateOTRMaterialCommand request, CancellationToken cancellationToken)
        {
            var otrMaterial = new OTRMaterial
            {
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),
                OrdenTrabajoCode = request.OrdenTrabajoCode.Trim().ToUpperInvariant(),
                ArticuloCode = request.ArticuloCode.Trim().ToUpperInvariant(),
                FechaProceso = request.FechaProceso ?? DateTime.UtcNow,
                Cantidad = request.Cantidad,
                CostoUnitario = request.CostoUnitario,
                CostoTotal = request.CostoTotal,
                EstadoCode = string.IsNullOrWhiteSpace(request.EstadoCode) ? null : request.EstadoCode.Trim().ToUpperInvariant()
            };

            await _uow.Mantenimiento.Transacciones.OTRMateriales.AddAsync(otrMaterial, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(otrMaterial);
        }

        internal static OTRMaterialResponse Map(OTRMaterial o) => new()
        {
            PlantaCode = o.PlantaCode,
            OrdenTrabajoCode = o.OrdenTrabajoCode,
            FechaProceso = o.FechaProceso,
            ArticuloCode = o.ArticuloCode,
            Cantidad = o.Cantidad,
            CostoUnitario = o.CostoUnitario,
            CostoTotal = o.CostoTotal,
            EstadoCode = o.EstadoCode,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
            RowVersion = o.RowVersion
        };
    }
}
