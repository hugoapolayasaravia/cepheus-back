// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/GetImportacionesPaginated/GetImportacionesPaginatedQuery.cs
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GetImportacionesPaginated
{
    /// <summary>
    /// Parámetros calcados de Logi_sp_Listado_MImportaciones
    /// (@Codigo_Pla, @Codigo_Imp, @Codigo_Est, @Fecha_Ini, @Fecha_Fin).
    /// El centinela legacy 'T' (todos) se representa con null/vacío (también se acepta "T").
    /// Regla del SP: si viene Code se busca exacto y se ignora el rango de fechas.
    /// </summary>
    public class GetImportacionesPaginatedQuery : PagedRequest, IRequest<PagedResult<ImportacionResponse>>
    {
        public string? PlantaCode { get; set; }
        public string? Code { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
