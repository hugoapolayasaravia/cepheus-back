using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetSiguienteDocumentoOrdenTrabajoVenta;

/// <summary>Vista previa de la numeración de una planta (PB: fc_jaladoc). No reserva el número.</summary>
public sealed record GetSiguienteDocumentoQuery(string Planta, TipoDocumentoAprobacion Tipo)
    : IRequest<SiguienteDocumentoResponse>;

public sealed class SiguienteDocumentoResponse
{
    public string PlantCode { get; set; } = default!;
    public string PlantName { get; set; } = default!;
    public TipoDocumentoAprobacion DocumentType { get; set; }

    /// <summary>Último número usado por la planta.</summary>
    public string LastNumber { get; set; } = default!;

    /// <summary>Número que se asignaría a la próxima aprobación al Contado.</summary>
    public string NextNumber { get; set; } = default!;
}
