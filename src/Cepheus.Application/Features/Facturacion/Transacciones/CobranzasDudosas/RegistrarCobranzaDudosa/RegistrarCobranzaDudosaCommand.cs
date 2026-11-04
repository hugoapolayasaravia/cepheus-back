using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.RegistrarCobranzaDudosa
{
    /// <summary>
    /// Envía un documento a cobranza dudosa (legacy w_con_cobranzadudosa, ue_set_btn_grb_data): lo registra con el saldo
    /// del documento y el tipo de cambio venta de la fecha de proceso, y lo marca como Dudosa. El asiento de canje se
    /// genera aparte. Tipos: 01, 03, 08, 10 y 11. Planta: 08 y 10. Cliente y banco: cheques.
    /// </summary>
    public class RegistrarCobranzaDudosaCommand : IRequest<CobranzaDudosaResponse>
    {
        public string TypeCode { get; set; } = default!;
        public string? PlantaCode { get; set; }
        public string Number { get; set; } = default!;
        public string? ClienteCode { get; set; }
        public string? BancoCode { get; set; }

        /// <summary>Fecha de proceso (Fecha_pro): fecha del asiento de canje y del tipo de cambio.</summary>
        public DateTime ProcessDate { get; set; }

        /// <summary>Tipo de contabilidad del asiento de canje (legacy Tipo_con; por defecto T = Tributaria en la ventana).</summary>
        public TipoContabilidad TipoContabilidad { get; set; } = TipoContabilidad.Tributaria;
    }
}
