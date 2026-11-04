using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.GenerarCanjeCobranzaDudosa
{
    /// <summary>
    /// Genera el asiento de canje a cobranza dudosa (legacy ue_set_generaasiento + fc_cobranzadudosacanje +
    /// fc_generadestino) con la plantilla activa del proceso CanjeCobranzaDudosa. El asiento reemplaza al de provisión
    /// del documento (el original queda guardado en el registro para restaurarlo al eliminar). El período contable de
    /// la fecha de proceso debe estar abierto.
    /// </summary>
    public record GenerarCanjeCobranzaDudosaCommand(
        string TypeCode, string? PlantaCode, string Number, string? ClienteCode, string? BancoCode, byte[] RowVersion)
        : IRequest<CobranzaDudosaResponse>;
}
