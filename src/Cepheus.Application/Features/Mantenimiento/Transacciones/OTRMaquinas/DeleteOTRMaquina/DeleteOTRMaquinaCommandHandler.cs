using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.DeleteOTRMaquina
{
    public class DeleteOTRMaquinaCommandHandler : IRequestHandler<DeleteOTRMaquinaCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteOTRMaquinaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteOTRMaquinaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var ordenCode = request.OrdenTrabajoCode.Trim().ToUpperInvariant();
            var maquinaCode = request.MaquinaCode.Trim().ToUpperInvariant();

            var otrMaquina = await _uow.Mantenimiento.Transacciones.OTRMaquinas.Query()
                .FirstOrDefaultAsync(o =>
                    o.PlantaCode == plantaCode &&
                    o.OrdenTrabajoCode == ordenCode &&
                    o.MaquinaCode == maquinaCode,
                    cancellationToken);

            if (otrMaquina is null)
            {
                throw new KeyNotFoundException($"Registro de máquina {plantaCode}/{ordenCode}/{maquinaCode} no encontrado.");
            }

            _uow.Mantenimiento.Transacciones.OTRMaquinas.Remove(otrMaquina);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
