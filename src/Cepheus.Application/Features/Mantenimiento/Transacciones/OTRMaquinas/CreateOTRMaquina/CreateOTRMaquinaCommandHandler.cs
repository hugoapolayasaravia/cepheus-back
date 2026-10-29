using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.Common;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.CreateOTRMaquina
{
    public class CreateOTRMaquinaCommandHandler : IRequestHandler<CreateOTRMaquinaCommand, OTRMaquinaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateOTRMaquinaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OTRMaquinaResponse> Handle(CreateOTRMaquinaCommand request, CancellationToken cancellationToken)
        {
            var otrMaquina = new OTRMaquina
            {
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),
                OrdenTrabajoCode = request.OrdenTrabajoCode.Trim().ToUpperInvariant(),
                MaquinaCode = request.MaquinaCode.Trim().ToUpperInvariant(),
                FechaProceso = request.FechaProceso ?? DateTime.UtcNow,
                Cantidad = request.Cantidad,
                Horas = request.Horas
            };

            await _uow.Mantenimiento.Transacciones.OTRMaquinas.AddAsync(otrMaquina, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(otrMaquina);
        }

        internal static OTRMaquinaResponse Map(OTRMaquina o) => new()
        {
            PlantaCode = o.PlantaCode,
            OrdenTrabajoCode = o.OrdenTrabajoCode,
            MaquinaCode = o.MaquinaCode,
            FechaProceso = o.FechaProceso,
            Cantidad = o.Cantidad,
            Horas = o.Horas,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
            RowVersion = o.RowVersion
        };
    }
}
