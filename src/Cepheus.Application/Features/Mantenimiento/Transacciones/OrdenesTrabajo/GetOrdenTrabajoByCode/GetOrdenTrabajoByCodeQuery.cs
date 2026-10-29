using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.GetOrdenTrabajoByCode
{
    public record GetOrdenTrabajoByCodeQuery(string PlantaCode, string Code) : IRequest<OrdenTrabajoResponse>;
}
