using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.GenerarAsientoOrdenServicio
{
    public class GenerarAsientoOrdenServicioCommandHandler
        : IRequestHandler<GenerarAsientoOrdenServicioCommand, AsientoOrdenServicioResultResponse>
    {
        private readonly IUnitOfWork _uow;

        public GenerarAsientoOrdenServicioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<AsientoOrdenServicioResultResponse> Handle(
            GenerarAsientoOrdenServicioCommand request, CancellationToken cancellationToken)
            => AsientoOrdenServicioGenerator.GenerateAsync(
                _uow, request.PlantaCode, request.OrdenServicioCode, cancellationToken);
    }
}
