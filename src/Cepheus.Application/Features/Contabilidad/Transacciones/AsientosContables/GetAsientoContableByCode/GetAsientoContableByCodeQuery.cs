using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.GetAsientoContableByCode
{
    /// <summary>Codigo = año(4) + mes(2) + tipo(2) + número(5).</summary>
    public record GetAsientoContableByCodeQuery(string Codigo) : IRequest<AsientoContableResponse>;
}
