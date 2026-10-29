namespace Cepheus.Application.Features.Logistica.Transacciones.Transferencias.Common;

/// <summary>
/// Constantes del ingreso por transferencia entre almacenes (valores que el PowerBuilder fijaba en ue_set_btn_transferir
/// y ue_transferir de la ventana Maestro de Nota de Ingresos).
/// </summary>
public static class TransferenciaRules
{
    /// <summary>Comprobante "NINGUNO" (codigo_tdo '00').</summary>
    public const string ComprobanteNinguno = "00";

    /// <summary>Forma de pago "SEGUN NOTAS" (codigo_pgc '10').</summary>
    public const string FormaPagoSegunNotas = "01";

    /// <summary>Prefijo del número de documento: 'TRANSFERENCIA' + código de la planta origen.</summary>
    public const string PrefijoNumeroDocumento = "TRANSFERENCIA";

    public static string NumeroDocumento(string plantaOrigenCode) => PrefijoNumeroDocumento + plantaOrigenCode;
}
