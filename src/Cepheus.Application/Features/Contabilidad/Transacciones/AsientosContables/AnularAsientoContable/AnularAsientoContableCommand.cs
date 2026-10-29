using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.AnularAsientoContable
{
    public record AnularAsientoContableCommand(string Codigo) : IRequest<AsientoContableResponse>;
}
