using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.Transportistas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cepheus.Application.Features.Logistica.Maestros.Transportistas.GetTransportistasPaginated
{
    public class GetTransportistasPaginatedQueryHandler
        : IRequestHandler<GetTransportistasPaginatedQuery, PagedResult<TransportistaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTransportistasPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<TransportistaResponse>> Handle(
            GetTransportistasPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Maestros.Transportistas.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t =>
                    t.Code.ToLower().Contains(search) ||
                    t.DocumentNumber.ToLower().Contains(search) ||
                    t.LegalName.ToLower().Contains(search) ||
                    (t.TradeName != null && t.TradeName.ToLower().Contains(search)));
            }

            if (request.IsOwnFleet.HasValue)
            {
                query = query.Where(t => t.IsOwnFleet == request.IsOwnFleet.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(t => new TransportistaResponse
            {
                Code = t.Code,
                DocumentTypeCode = t.DocumentTypeCode,
                DocumentNumber = t.DocumentNumber,
                LegalName = t.LegalName,
                TradeName = t.TradeName,
                Address = t.Address,
                UbigeoCode = t.UbigeoCode,
                Phone = t.Phone,
                Email = t.Email,
                MtcRegistrationNumber = t.MtcRegistrationNumber,
                CertificationNumber = t.CertificationNumber,
                IsOwnFleet = t.IsOwnFleet,
                Observations = t.Observations,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedBy = t.UpdatedBy,
                RowVersion = t.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
