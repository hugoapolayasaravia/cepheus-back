using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.Articulos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Articulos.GetArticulosPaginated
{
    public class GetArticulosPaginatedQueryHandler
        : IRequestHandler<GetArticulosPaginatedQuery, PagedResult<ArticuloResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetArticulosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ArticuloResponse>> Handle(
            GetArticulosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Maestros.Articulos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(a =>
                    a.Code.ToLower().Contains(search) ||
                    a.Name.ToLower().Contains(search) ||
                    (a.ManufacturerCode != null && a.ManufacturerCode.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(request.SubFamiliaCode))
            {
                var subFamiliaCode = request.SubFamiliaCode.Trim().ToUpper();
                query = query.Where(a => a.SubFamiliaCode == subFamiliaCode);
            }

            if (!string.IsNullOrWhiteSpace(request.TipoArticuloCode))
            {
                var tipoArticuloCode = request.TipoArticuloCode.Trim().ToUpper();
                query = query.Where(a => a.TipoArticuloCode == tipoArticuloCode);
            }

            if (request.AbcClass.HasValue)
            {
                query = query.Where(a => a.AbcClass == request.AbcClass.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(a => a.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(a => a.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(a => new ArticuloResponse
            {
                Code = a.Code,
                Name = a.Name,
                UnidadMedidaCode = a.UnidadMedidaCode,
                SubFamiliaCode = a.SubFamiliaCode,
                SubFamiliaName = a.SubFamilia.Name,
                UnidadMedidaName = a.UnidadMedida.Name,
                MinStock = a.MinStock,
                MaxStock = a.MaxStock,
                IncomingStock = a.IncomingStock,
                LeadTimeDays = a.LeadTimeDays,
                AbcClass = a.AbcClass,
                TipoArticuloCode = a.TipoArticuloCode,
                PlanCode = a.PlanCode,
                ManufacturerCode = a.ManufacturerCode,
                Observations = a.Observations,
                SalesTypeCode = a.SalesTypeCode,
                SalesProductCode = a.SalesProductCode,
                IsAgreement = a.IsAgreement,
                AccountingAccountCode = a.AccountingAccountCode,
                AccountingAttachmentTypeCode = a.AccountingAttachmentTypeCode,
                PlantOriginCode = a.PlantOriginCode,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                CreatedBy = a.CreatedBy,
                UpdatedBy = a.UpdatedBy,
                RowVersion = a.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
