using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.EliminarAsientoOrdenServicio
{
    public record EliminarAsientoOrdenServicioCommand(string PlantaCode, string OrdenServicioCode)
        : IRequest<AsientoOrdenServicioResultResponse>;
}
