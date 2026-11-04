using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.CastigarCobranzaDudosa
{
    /// <summary>
    /// Castiga el documento en cobranza dudosa (legacy ue_gen_castigo): lo marca Castigado y guarda la fecha de castigo.
    /// No genera asiento. Requiere el asiento de canje ya generado.
    /// </summary>
    public record CastigarCobranzaDudosaCommand(
        string TypeCode, string? PlantaCode, string Number, string? ClienteCode, string? BancoCode, byte[] RowVersion)
        : IRequest<CobranzaDudosaResponse>;
}
