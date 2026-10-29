using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.GenerarAsientoConsumo
{
    public class GenerarAsientoConsumoCommandHandler
        : IRequestHandler<GenerarAsientoConsumoCommand, AsientoConsumoResultResponse>
    {
        private readonly IUnitOfWork _uow;

        public GenerarAsientoConsumoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<AsientoConsumoResultResponse> Handle(
            GenerarAsientoConsumoCommand request, CancellationToken cancellationToken)
            => AsientoConsumoGenerator.GenerateAsync(
                _uow, request.PlantaCode, request.FechaDesde, request.FechaHasta, cancellationToken);
    }
}
