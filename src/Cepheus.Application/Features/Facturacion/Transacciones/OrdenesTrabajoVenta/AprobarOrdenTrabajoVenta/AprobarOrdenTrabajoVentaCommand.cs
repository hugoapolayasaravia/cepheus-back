using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AprobarOrdenTrabajoVenta;

/// <summary>Datos de la aprobación manual de Créditos (PB: w_ges_otrabajo_aprobacion).</summary>
public sealed class AprobarOrdenTrabajoVentaRequest
{
    /// <summary>Aprobado o Rechazado (Estado_cre). Rechazado deja las cantidades aprobadas en 0.</summary>
    public EstadoCreditoAprobacion CreditStatus { get; set; } = EstadoCreditoAprobacion.Aprobado;

    /// <summary>Contado (lleva documento de pago) o Crédito (sin documento).</summary>
    public TipoPagoAprobacion PaymentType { get; set; }

    /// <summary>Factura o Boleta. Si se omite: Factura con RUC de 11 dígitos, Boleta en otro caso. Factura exige RUC.</summary>
    public TipoDocumentoAprobacion? DocumentType { get; set; }

    /// <summary>Tipo de comprobante aprobado (codigo_tip). Si se omite: Contado => TipoP, Crédito => SinDocumento. Distinto de SinDocumento solo en Contado.</summary>
    public FlujoDocumentoAprobacion? DocumentFlow { get; set; }

    /// <summary>Código de tipo de valorización del catálogo, o "N" (ninguna). Si se omite se toma el de la obra.</summary>
    public string? ValorizationCode { get; set; }

    /// <summary>Valorización semanal (S) exige día de la semana; por días (O) exige días &gt; 0. Si faltan se toman de la obra / tipo de valorización.</summary>
    public DiaSemana? ValorizationWeekday { get; set; }
    public int? ValorizationDays { get; set; }

    /// <summary>Si se omite: la forma de pago de la obra.</summary>
    public string? FormaPagoVentaCode { get; set; }

    /// <summary>Planta cuyo correlativo da el número del documento (obligatoria en Contado).</summary>
    public string? Plant1Code { get; set; }

    /// <summary>Doble documento (solo Factura con líneas de bombeo/transporte): el segundo debe ser distinto del primero.</summary>
    public bool HasTwoDocuments { get; set; }
    public string? Plant2Code { get; set; }

    /// <summary>Números "SSS-NNNNNN". Si se omiten, el sistema asigna el siguiente de la planta.</summary>
    public string? Document1Number { get; set; }
    public string? Document2Number { get; set; }

    public string? GuideMotivoCode { get; set; }

    /// <summary>Forma de cobro (Codigo_tpa). Obligatoria en Contado; 01 = efectivo (sin banco ni N°), 04 y 10 sin banco.</summary>
    public string? PaymentMethodCode { get; set; }

    /// <summary>Banco y N° de pago según la forma de cobro.</summary>
    public string? BankCode { get; set; }
    public string? PaymentDocumentNumber { get; set; }

    public string? CreditsObservations { get; set; }
    public string? ApprovalObservations { get; set; }

    /// <summary>Obligatorio si pasó 1 día o más desde que la OT se envió a aprobación.</summary>
    public string? DelayReason { get; set; }

    /// <summary>Pasa a producción al aprobar. Si se omite: no en Concreto al Contado; sí en los demás casos.</summary>
    public bool? PassToProduction { get; set; }

    /// <summary>Motivo cuando no pasa a producción.</summary>
    public string? ProductionPassReason { get; set; }
}

public sealed record AprobarOrdenTrabajoVentaCommand(string Code, AprobarOrdenTrabajoVentaRequest Data)
    : IRequest<OrdenTrabajoVentaResponse>;
