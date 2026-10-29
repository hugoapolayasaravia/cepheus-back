using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.Common;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.CreateOTResponsable
{
    public class CreateOTResponsableCommandHandler : IRequestHandler<CreateOTResponsableCommand, OTResponsableResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateOTResponsableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OTResponsableResponse> Handle(CreateOTResponsableCommand request, CancellationToken cancellationToken)
        {
            var otResponsable = new OTResponsable
            {
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),
                OrdenTrabajoCode = request.OrdenTrabajoCode.Trim().ToUpperInvariant(),
                FechaProceso = request.FechaProceso ?? DateTime.UtcNow,
                TrabajadorCode = request.TrabajadorCode.Trim().ToUpperInvariant(),
                TiempoProceso = request.TiempoProceso,
                Basico = request.Basico,
                CostoTotal = request.CostoTotal
            };

            await _uow.Mantenimiento.Transacciones.OTResponsables.AddAsync(otResponsable, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(otResponsable);
        }

        internal static OTResponsableResponse Map(OTResponsable o) => new()
        {
            PlantaCode = o.PlantaCode,
            OrdenTrabajoCode = o.OrdenTrabajoCode,
            FechaProceso = o.FechaProceso,
            TrabajadorCode = o.TrabajadorCode,
            TiempoProceso = o.TiempoProceso,
            Basico = o.Basico,
            CostoTotal = o.CostoTotal,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
            RowVersion = o.RowVersion
        };
    }
}
