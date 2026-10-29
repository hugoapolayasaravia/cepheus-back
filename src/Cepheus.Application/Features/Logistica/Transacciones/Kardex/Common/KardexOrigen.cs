using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Transacciones.Kardex.Common;

/// <summary>Códigos de origen de 2 letras del Kardex legacy (columna origen_fue del DataWindow).</summary>
public static class KardexOrigen
{
    public static string Codigo(OrigenMovimientoArticulo origen) => origen switch
    {
        OrigenMovimientoArticulo.NotaIngreso => "NI",
        OrigenMovimientoArticulo.NotaCredito => "NI",
        OrigenMovimientoArticulo.Importacion => "NI",
        OrigenMovimientoArticulo.TransferenciaIngreso => "NI",
        OrigenMovimientoArticulo.OrdenServicio => "OS",
        OrigenMovimientoArticulo.ValeSalida => "VS",
        OrigenMovimientoArticulo.AjusteIngreso => "AI",
        OrigenMovimientoArticulo.AjusteSalida => "AS",
        OrigenMovimientoArticulo.AjusteManual => "AM",
        OrigenMovimientoArticulo.GuiaIngreso => "GI",
        OrigenMovimientoArticulo.Apertura => "AP",
        _ => "??"
    };

    /// <summary>El DataWindow pinta en rojo los ajustes de inventario (AI / AS).</summary>
    public static bool EsAjuste(OrigenMovimientoArticulo origen) =>
        origen is OrigenMovimientoArticulo.AjusteIngreso
               or OrigenMovimientoArticulo.AjusteSalida
               or OrigenMovimientoArticulo.AjusteManual;
}
