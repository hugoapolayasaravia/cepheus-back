using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.DeleteCobranzaDudosa
{
    /// <summary>
    /// Elimina la cobranza dudosa de un documento (legacy ue_set_btn_eli_bor): el documento vuelve a Normal.
    /// Si ya tiene asiento de canje, lo anula (período abierto) y el documento recupera su asiento de provisión original.
    /// No se elimina si el registro está Cancelado ni si el documento ya tiene cancelaciones aplicadas como dudoso.
    /// </summary>
    public record DeleteCobranzaDudosaCommand(
        string TypeCode, string? PlantaCode, string Number, string? ClienteCode, string? BancoCode) : IRequest<Unit>;
}
