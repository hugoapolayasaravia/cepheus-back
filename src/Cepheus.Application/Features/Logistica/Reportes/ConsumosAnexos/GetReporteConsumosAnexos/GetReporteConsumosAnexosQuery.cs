using MediatR;

namespace Cepheus.Application.Features.Logistica.Reportes.ConsumosAnexos.GetReporteConsumosAnexos;

/// <summary>
/// Consumos x Anexos / Consumo Gerencial (legacy Logi_sp_Rpt_Consumo_Gerencial): consumos procesados de Vales de Salida
/// y de salidas por Orden de Servicio, por fecha de entrega, con la cuenta contable y el anexo al que se imputan
/// (enlace subcentro + subfamilia, entidad SubFamiliaAnexo). Los filtros en null equivalen a "T" (todos).
/// </summary>
/// <param name="PlantaCode">Planta del documento (obligatoria, como Codigo_Pla del legacy).</param>
/// <param name="PlantaAfectadaCode">Planta afectada por el consumo (@plt del legacy).</param>
/// <param name="AccountingAccountCode">Cuenta contable del subcentro de costo.</param>
/// <param name="AccountingAttachmentTypeCode">Tipo de anexo del subcentro de costo.</param>
/// <param name="AccountingAttachmentCode">Anexo del enlace subcentro + subfamilia.</param>
public sealed record GetReporteConsumosAnexosQuery(
    string PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    string? AccountingAccountCode,
    string? AccountingAttachmentTypeCode,
    string? AccountingAttachmentCode,
    string? CentroCostoCode,
    string? SubCentroCostoCode,
    string? PlantaAfectadaCode) : IRequest<ReporteConsumosAnexosResponse>;
