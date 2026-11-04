using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.GetCobranzaDudosa
{
    /// <summary>Cobranza dudosa de un documento (legacy dw_cab_cobranzadudosa).</summary>
    public record GetCobranzaDudosaQuery(
        string TypeCode, string? PlantaCode, string Number, string? ClienteCode, string? BancoCode)
        : IRequest<CobranzaDudosaResponse>;
}
