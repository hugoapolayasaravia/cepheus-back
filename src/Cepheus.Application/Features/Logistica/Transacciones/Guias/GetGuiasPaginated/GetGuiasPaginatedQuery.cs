// Cepheus.Application/Features/Logistica/Transacciones/Guias/GetGuiasPaginated/GetGuiasPaginatedQuery.cs
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.GetGuiasPaginated
{
    /// <summary>
    /// Parámetros del SP legacy Logi_sp_Listado_MGuias: @Codigo_Pla,
    /// @Serie, @Guia, @Codigo_Cli (proveedor), @Codigo_Est, @Fecha_Ini y
    /// @Fecha_Fin. Igual que el legacy, el valor "T" (o vacío) significa
    /// "todos" en Planta, Serie, Guia, Proveedor y Estado.
    ///
    /// Semántica del legacy que se conserva: si se indica una Guia concreta,
    /// se busca solo por ese número y se ignoran Serie, Proveedor, Estado y
    /// fechas. Si Guia es "T", se aplican el rango de fechas y los demás
    /// filtros. Search es un filtro de texto libre adicional del patrón del
    /// repositorio (número de guía, observaciones o nombre del proveedor).
    ///
    /// Orden por defecto: más reciente primero (CreatedAt, Code), igual que
    /// el ORDER BY del legacy (FecProceso_gve DESC, Numero_gve DESC).
    /// </summary>
    public class GetGuiasPaginatedQuery : PagedRequest, IRequest<PagedResult<GuiaListItemResponse>>
    {
        public string? Search { get; set; }

        /// <summary>Planta (Codigo_Pla). "T" o vacío = todas.</summary>
        public string? PlantaCode { get; set; }

        /// <summary>Serie de 3 dígitos (@Serie). "T" o vacío = todas.</summary>
        public string? Serie { get; set; }

        /// <summary>Número de guía completo 'SSS-NNNNNN' (@Guia). "T" o vacío = todas.</summary>
        public string? Guia { get; set; }

        /// <summary>Código de proveedor destinatario (@Codigo_Cli). "T" o vacío = todos.</summary>
        public string? ProveedorCode { get; set; }

        /// <summary>Pendiente | Anulado (@Codigo_Est). "T" o vacío = todos.</summary>
        public string? Estado { get; set; }

        /// <summary>Fecha de emisión desde (@Fecha_Ini), inclusive.</summary>
        public DateTime? FechaInicio { get; set; }

        /// <summary>Fecha de emisión hasta (@Fecha_Fin), inclusive.</summary>
        public DateTime? FechaFin { get; set; }
    }
}
