using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GenerarLetrasVarias
{
    /// <summary>Documento que se refinancia con las letras (legacy: fila marcada de la lista de pendientes).</summary>
    public class LetrasVariasDocumentoInput
    {
        /// <summary>01 factura, 03 boleta, 07 nota de crédito, 08 nota de débito, 10 letra, 11 cheque rebotado.</summary>
        public string TypeCode { get; set; } = default!;
        public string? PlantaCode { get; set; }
        public string Number { get; set; } = default!;

        /// <summary>Solo cheques: banco del cheque.</summary>
        public string? BancoCode { get; set; }

        /// <summary>
        /// Importe a refinanciar, SIEMPRE POSITIVO y en la moneda de las letras (legacy: pendiente_s_ori / pendiente_d_ori).
        /// Si no se indica se toma todo el saldo. Las notas de crédito restan del total.
        /// </summary>
        public decimal? Amount { get; set; }
    }

    /// <summary>
    /// Genera letras varias (legacy w_fac_reg_letras, evento ue_letras_varias, tipo de pago 11): refinancia documentos
    /// del cliente en N letras. Crea la cancelación (tipo 11), sus líneas y las letras (Facturado, sin asiento: el asiento
    /// y su marca de "emitida" los genera después el módulo de Cancelaciones). Con <c>PreviewOnly</c> solo calcula las
    /// cuotas (legacy: botón b_ok) y no guarda nada.
    ///
    /// Cuotas: (total - inicial) / cantidad, vencimientos cada 30 días desde la primera fecha; la última cuota
    /// absorbe el redondeo. El interés solo aplica con tipo de cuota "I" y se reparte igual.
    /// </summary>
    public record GenerarLetrasVariasCommand(
        string PlantaCode,
        string ClienteCode,
        DateTime Date,
        string CobradorCode,
        string MonedaCode,
        int InstallmentCount,
        string InstallmentType,
        DateTime FirstPaymentDate,
        decimal InitialAmount,
        decimal Interest,
        string? Description,
        List<LetrasVariasDocumentoInput> Documents,
        bool PreviewOnly = false
    ) : IRequest<GenerarLetrasVariasResponse>;

    public class LetraVariaCuotaResponse
    {
        public string? Number { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Interest { get; set; }
    }

    public class GenerarLetrasVariasResponse
    {
        public bool Saved { get; set; }
        public string? CancelacionCode { get; set; }
        public string MonedaCode { get; set; } = default!;
        public decimal ExchangeRate { get; set; }
        public decimal TotalDocuments { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal TotalLetras { get; set; }
        public List<LetraVariaCuotaResponse> Letras { get; set; } = new();
    }
}
