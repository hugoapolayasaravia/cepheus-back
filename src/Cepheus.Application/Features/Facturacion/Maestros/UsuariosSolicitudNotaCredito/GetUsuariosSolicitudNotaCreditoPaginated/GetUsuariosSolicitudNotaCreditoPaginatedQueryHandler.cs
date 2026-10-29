using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.GetUsuariosSolicitudNotaCreditoPaginated
{
    public class GetUsuariosSolicitudNotaCreditoPaginatedQueryHandler
        : IRequestHandler<GetUsuariosSolicitudNotaCreditoPaginatedQuery, PagedResult<UsuarioSolicitudNotaCreditoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetUsuariosSolicitudNotaCreditoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<UsuarioSolicitudNotaCreditoResponse>> Handle(
            GetUsuariosSolicitudNotaCreditoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.NegocioCode))
            {
                var negocio = request.NegocioCode.Trim().ToUpperInvariant();
                query = query.Where(u => u.NegocioCode == negocio);
            }

            if (!string.IsNullOrWhiteSpace(request.Stage))
            {
                var stage = EtapaSolicitudNotaCreditoParser.Parse(request.Stage);
                query = query.Where(u => u.Stage == stage);
            }

            if (request.UserId.HasValue)
            {
                query = query.Where(u => u.UserId == request.UserId.Value);
            }

            if (request.IsPrincipal.HasValue)
            {
                query = query.Where(u => u.IsPrincipal == request.IsPrincipal.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(u =>
                    u.User.FirstName.ToLower().Contains(search) ||
                    u.User.LastName.ToLower().Contains(search) ||
                    u.User.Email.ToLower().Contains(search) ||
                    u.Negocio.Name.ToLower().Contains(search));
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(u => u.NegocioCode).ThenBy(u => u.Stage).ThenBy(u => u.UserId)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(UsuarioSolicitudNotaCreditoProjection.ToResponse);

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
