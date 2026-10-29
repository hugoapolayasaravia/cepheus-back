using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.Common;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.UpdateOTRMaquina
{
    public class UpdateOTRMaquinaCommandHandler : IRequestHandler<UpdateOTRMaquinaCommand, OTRMaquinaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateOTRMaquinaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OTRMaquinaResponse> Handle(UpdateOTRMaquinaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var ordenCode = request.OrdenTrabajoCode.Trim().ToUpperInvariant();
            var maquinaCode = request.MaquinaCode.Trim().ToUpperInvariant();

            var current = await _uow.Mantenimiento.Transacciones.OTRMaquinas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(o =>
                    o.PlantaCode == plantaCode &&
                    o.OrdenTrabajoCode == ordenCode &&
                    o.MaquinaCode == maquinaCode,
                    cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Registro de máquina {plantaCode}/{ordenCode}/{maquinaCode} no encontrado.");
            }

            var otrMaquina = new OTRMaquina
            {
                PlantaCode = plantaCode,
                OrdenTrabajoCode = ordenCode,
                MaquinaCode = maquinaCode,

                FechaProceso = request.FechaProceso,
                Cantidad = request.Cantidad,
                Horas = request.Horas,

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Transacciones.OTRMaquinas.Update(otrMaquina);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateOTRMaquina.CreateOTRMaquinaCommandHandler.Map(otrMaquina);
        }
    }
}
