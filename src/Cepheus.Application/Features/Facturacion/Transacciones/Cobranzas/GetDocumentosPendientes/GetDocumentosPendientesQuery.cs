using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cobranzas.GetDocumentosPendientes
{
    /// <summary>
    /// Documentos por cobrar de un cliente (legacy cr_consulta_documentos): facturas (01), boletas (03), notas de
    /// crédito (07), notas de débito (08), letras (10) y cheques rebotados (11). <c>TypeCode</c> "00" o vacío trae todos.
    /// Solo aparecen los documentos Facturado (por cobrar), emitidos, con asiento de provisión y que no estén ya
    /// incluidos en unas letras (las facturas, boletas y notas de crédito marcadas en CashDocument).
    /// </summary>
    public class GetDocumentosPendientesQuery : IRequest<List<DocumentoPendienteResponse>>
    {
        public string? ClienteCode { get; set; }
        public string? TypeCode { get; set; }
        public string? PlantaCode { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class DocumentoPendienteResponse
    {
        public string TypeCode { get; set; } = default!;
        public string? PlantaCode { get; set; }
        public string Number { get; set; } = default!;

        /// <summary>Solo cheques: banco (forma parte de la clave del cheque).</summary>
        public string? BancoCode { get; set; }

        public string ClienteCode { get; set; } = default!;
        public DateTime IssueDate { get; set; }

        /// <summary>Vencimiento: en letras y cheques su fecha de pago; en el resto emisión + días de la forma de pago.</summary>
        public DateTime DueDate { get; set; }

        public string CurrencyCode { get; set; } = default!;
        public decimal ExchangeRate { get; set; }

        /// <summary>Total y saldo en la moneda del documento. Las notas de crédito van en negativo, como el legacy.</summary>
        public decimal Total { get; set; }
        public decimal Pending { get; set; }

        public string? ObraCode { get; set; }
        public string? ProvisionEntryNumber { get; set; }
    }
}
