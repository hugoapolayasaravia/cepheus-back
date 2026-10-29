using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.GetSubFamiliaAnexosPaginated;

public class GetSubFamiliaAnexosPaginatedQueryHandler
    : IRequestHandler<GetSubFamiliaAnexosPaginatedQuery, PagedResult<SubFamiliaAnexoResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetSubFamiliaAnexosPaginatedQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<SubFamiliaAnexoResponse>> Handle(
        GetSubFamiliaAnexosPaginatedQuery request, CancellationToken ct)
    {
        var query = _uow.Logistica.Maestros.SubFamiliaAnexos.Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x =>
                x.SubCentroCostoCode.ToLower().Contains(search) ||
                x.SubCentroCosto.Name.ToLower().Contains(search) ||
                x.SubFamiliaCode.ToLower().Contains(search) ||
                x.SubFamilia.Name.ToLower().Contains(search) ||
                x.AccountingAccountCode.ToLower().Contains(search) ||
                x.CuentaContable.Name.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(request.CentroCostoCode))
        {
            var cc = request.CentroCostoCode.Trim().ToUpper();
            query = query.Where(x => x.SubCentroCosto.CentroCostoCode == cc);
        }

        if (!string.IsNullOrWhiteSpace(request.SubCentroCostoCode))
        {
            var scc = request.SubCentroCostoCode.Trim().ToUpper();
            query = query.Where(x => x.SubCentroCostoCode == scc);
        }

        if (!string.IsNullOrWhiteSpace(request.SubFamiliaCode))
        {
            var sfa = request.SubFamiliaCode.Trim().ToUpper();
            query = query.Where(x => x.SubFamiliaCode == sfa);
        }

        if (!string.IsNullOrWhiteSpace(request.AccountingAccountCode))
        {
            var cta = request.AccountingAccountCode.Trim();
            query = query.Where(x => x.AccountingAccountCode == cta);
        }

        if (!string.IsNullOrWhiteSpace(request.AccountingAttachmentTypeCode))
        {
            var tip = request.AccountingAttachmentTypeCode.Trim();
            query = query.Where(x => x.AccountingAttachmentTypeCode == tip);
        }

        if (!string.IsNullOrWhiteSpace(request.AccountingAttachmentCode))
        {
            var ane = request.AccountingAttachmentCode.Trim();
            query = query.Where(x => x.AccountingAttachmentCode == ane);
        }

        var sortDesc = request.SortDesc ?? false;
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;

        var sorted = string.IsNullOrWhiteSpace(request.SortBy)
            ? query.OrderBy(x => x.SubCentroCostoCode).ThenBy(x => x.SubFamiliaCode)
            : query.ApplySort(request.SortBy, sortDesc);

        return await sorted.ToResponse().ToPagedResultAsync(pageNumber, pageSize, ct);
    }
}
