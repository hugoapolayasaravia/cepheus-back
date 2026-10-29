using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.GenerarAsientoOrdenServicio
{
    public record GenerarAsientoOrdenServicioCommand(string PlantaCode, string OrdenServicioCode)
        : IRequest<AsientoOrdenServicioResultResponse>;
}
