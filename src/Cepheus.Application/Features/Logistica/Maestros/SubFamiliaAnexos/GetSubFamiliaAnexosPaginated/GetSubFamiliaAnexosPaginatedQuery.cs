using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.GetSubFamiliaAnexosPaginated;

public class GetSubFamiliaAnexosPaginatedQuery : PagedRequest, IRequest<PagedResult<SubFamiliaAnexoResponse>>
{
    /// <summary>Busca en códigos y nombres de subcentro, subfamilia y cuenta contable.</summary>
    public string? Search { get; set; }
    public string? CentroCostoCode { get; set; }
    public string? SubCentroCostoCode { get; set; }
    public string? SubFamiliaCode { get; set; }
    public string? AccountingAccountCode { get; set; }
    public string? AccountingAttachmentTypeCode { get; set; }
    public string? AccountingAttachmentCode { get; set; }
}
