using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.GenerarAsientoNotaIngreso
{
    public record GenerarAsientoNotaIngresoCommand(string PlantaCode, string NotaIngresoCode)
        : IRequest<AsientoNotaIngresoResultResponse>;
}
