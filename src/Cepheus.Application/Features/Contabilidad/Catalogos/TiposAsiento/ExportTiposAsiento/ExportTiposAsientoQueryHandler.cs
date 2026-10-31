using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.Common;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.CreateTipoAsiento;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.ExportTiposAsiento
{
    public class ExportTiposAsientoQueryHandler : IRequestHandler<ExportTiposAsientoQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<TipoAsientoResponse>> Catalogo = new List<ExportColumn<TipoAsientoResponse>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("exchangeRateType", "Tipo de cambio aplicable", f => f.ExchangeRateType.ToString()),
            new("valuation", "Valorización", f => f.Valuation.ToString()),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "name", "exchangeRateType", "valuation", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportTiposAsientoQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportTiposAsientoQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Contabilidad.Catalogos.TiposAsiento.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(search) ||
                    x.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            if (request.ExchangeRateType.HasValue)
            {
                query = query.Where(x => x.ExchangeRateType == request.ExchangeRateType.Value);
            }

            if (request.Valuation.HasValue)
            {
                query = query.Where(x => x.Valuation == request.Valuation.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            //var proyeccion = sortedQuery.Select(CreateTipoAsientoCommandHandler.Map);

            //var filas = await proyeccion
            //    .Take(ExportBuilder.MaxFilas(formato) + 1)
            //    .ToListAsync(cancellationToken);

            var entidades = await sortedQuery
            .Take(ExportBuilder.MaxFilas(formato) + 1)
            .ToListAsync(cancellationToken);

            var filas = entidades
                .Select(CreateTipoAsientoCommandHandler.Map)
                .ToList();

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Tipos de asiento", "tipos_asiento", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    request.ExchangeRateType.HasValue ? $"Tipo de cambio aplicable: {request.ExchangeRateType.Value}" : null,
                    request.Valuation.HasValue ? $"Valorización: {request.Valuation.Value}" : null));
        }
    }
}
