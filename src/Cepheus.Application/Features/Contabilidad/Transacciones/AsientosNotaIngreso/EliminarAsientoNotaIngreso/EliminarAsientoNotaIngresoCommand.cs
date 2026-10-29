using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.EliminarAsientoNotaIngreso
{
    public record EliminarAsientoNotaIngresoCommand(string PlantaCode, string NotaIngresoCode)
        : IRequest<AsientoNotaIngresoResultResponse>;
}
