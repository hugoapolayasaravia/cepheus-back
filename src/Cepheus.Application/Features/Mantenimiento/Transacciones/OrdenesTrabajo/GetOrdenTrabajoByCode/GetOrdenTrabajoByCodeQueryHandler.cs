using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.GetOrdenTrabajoByCode
{
    public class GetOrdenTrabajoByCodeQueryHandler : IRequestHandler<GetOrdenTrabajoByCodeQuery, OrdenTrabajoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetOrdenTrabajoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenTrabajoResponse> Handle(GetOrdenTrabajoByCodeQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var ordenTrabajo = await _uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.PlantaCode == plantaCode && o.Code == code, cancellationToken);

            if (ordenTrabajo is null)
            {
                throw new KeyNotFoundException($"Orden de Trabajo {plantaCode}/{code} no encontrada.");
            }

            return await CreateOrdenTrabajo.CreateOrdenTrabajoCommandHandler.Map(_uow, ordenTrabajo, cancellationToken);
        }
    }
}
