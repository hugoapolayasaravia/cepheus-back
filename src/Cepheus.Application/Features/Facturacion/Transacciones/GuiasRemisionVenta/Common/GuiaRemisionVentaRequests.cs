namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;

/// <summary>Datos editables de la cabecera de la guía (transporte, destino y flete).</summary>
public class GuiaRemisionVentaFields
{
    /// <summary>Transportista (obligatorio).</summary>
    public string TransportistaCode { get; set; } = default!;

    /// <summary>Chofer del transportista (obligatorio).</summary>
    public string ChoferCode { get; set; } = default!;

    /// <summary>Tipo de vehículo (1 Camión, 2 Bomba/Mixer, 3 Carreta, 4 Otros); va junto con VehicleCode.</summary>
    public int? VehicleType { get; set; }

    public string? VehicleCode { get; set; }

    /// <summary>Dirección de entrega; si se omite se toma la de entrega de la obra.</summary>
    public string? Address { get; set; }

    /// <summary>Ubigeo de la dirección; si se omite se toma el de entrega de la obra.</summary>
    public string? UbigeoCode { get; set; }

    /// <summary>Descripción (obligatoria).</summary>
    public string Description { get; set; } = default!;

    /// <summary>Flete / zonificación (obligatorio).</summary>
    public string FleteCode { get; set; } = default!;

    /// <summary>Monto del flete; si se omite se toma el del catálogo.</summary>
    public decimal? FleteAmount { get; set; }

    /// <summary>Hora de despacho "HH:mm" (opcional).</summary>
    public string? DispatchTime { get; set; }

    public string? TicketNumber { get; set; }
    public string? PumpCode { get; set; }
}

public sealed class GuiaRemisionVentaDetalleRequest
{
    /// <summary>Item (Clave) de la línea de la OT. Obligatorio con OT; sin OT se envía producto.</summary>
    public int? Item { get; set; }

    /// <summary>Solo guías sin OT.</summary>
    public string? ProductoTipoCode { get; set; }

    /// <summary>Solo guías sin OT.</summary>
    public string? ProductoCode { get; set; }

    /// <summary>Cantidad (mayor a cero). Con programación: máximo lo programado pendiente y lo aprobado pendiente.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Solo guías sin OT (con OT se toma el precio de la línea).</summary>
    public decimal? UnitPrice { get; set; }
}
