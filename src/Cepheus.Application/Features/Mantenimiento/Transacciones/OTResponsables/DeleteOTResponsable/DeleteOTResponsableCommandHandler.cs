using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.DeleteOTResponsable
{
    public class DeleteOTResponsableCommandHandler : IRequestHandler<DeleteOTResponsableCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteOTResponsableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteOTResponsableCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var ordenCode = request.OrdenTrabajoCode.Trim().ToUpperInvariant();
            var trabajadorCode = request.TrabajadorCode.Trim().ToUpperInvariant();

            var otResponsable = await _uow.Mantenimiento.Transacciones.OTResponsables.Query()
                .FirstOrDefaultAsync(o =>
                    o.PlantaCode == plantaCode &&
                    o.OrdenTrabajoCode == ordenCode &&
                    o.FechaProceso == request.FechaProceso &&
                    o.TrabajadorCode == trabajadorCode,
                    cancellationToken);

            if (otResponsable is null)
            {
                throw new KeyNotFoundException(
                    $"Registro de responsable {plantaCode}/{ordenCode}/{trabajadorCode} en {request.FechaProceso:yyyy-MM-dd} no encontrado.");
            }

            _uow.Mantenimiento.Transacciones.OTResponsables.Remove(otResponsable);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
