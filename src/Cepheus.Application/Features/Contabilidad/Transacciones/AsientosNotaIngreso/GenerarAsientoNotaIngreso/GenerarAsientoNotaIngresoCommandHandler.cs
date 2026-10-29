using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.GenerarAsientoNotaIngreso
{
    public class GenerarAsientoNotaIngresoCommandHandler
        : IRequestHandler<GenerarAsientoNotaIngresoCommand, AsientoNotaIngresoResultResponse>
    {
        private readonly IUnitOfWork _uow;

        public GenerarAsientoNotaIngresoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<AsientoNotaIngresoResultResponse> Handle(
            GenerarAsientoNotaIngresoCommand request, CancellationToken cancellationToken)
            => AsientoNotaIngresoGenerator.GenerateAsync(
                _uow, request.PlantaCode, request.NotaIngresoCode, cancellationToken);
    }
}
