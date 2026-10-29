using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.Common;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.UpdateOTResponsable
{
    public class UpdateOTResponsableCommandHandler : IRequestHandler<UpdateOTResponsableCommand, OTResponsableResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateOTResponsableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OTResponsableResponse> Handle(UpdateOTResponsableCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var ordenCode = request.OrdenTrabajoCode.Trim().ToUpperInvariant();
            var trabajadorCode = request.TrabajadorCode.Trim().ToUpperInvariant();

            var current = await _uow.Mantenimiento.Transacciones.OTResponsables.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(o =>
                    o.PlantaCode == plantaCode &&
                    o.OrdenTrabajoCode == ordenCode &&
                    o.FechaProceso == request.FechaProceso &&
                    o.TrabajadorCode == trabajadorCode,
                    cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException(
                    $"Registro de responsable {plantaCode}/{ordenCode}/{trabajadorCode} en {request.FechaProceso:yyyy-MM-dd} no encontrado.");
            }

            var otResponsable = new OTResponsable
            {
                PlantaCode = plantaCode,
                OrdenTrabajoCode = ordenCode,
                FechaProceso = request.FechaProceso,
                TrabajadorCode = trabajadorCode,

                TiempoProceso = request.TiempoProceso,
                Basico = request.Basico,
                CostoTotal = request.CostoTotal,

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Transacciones.OTResponsables.Update(otResponsable);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateOTResponsable.CreateOTResponsableCommandHandler.Map(otResponsable);
        }
    }
}
