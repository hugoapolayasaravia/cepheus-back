using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.Fletes.ExportFletes
{
    public class ExportFletesQueryHandler : IRequestHandler<ExportFletesQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<FleteResponse>> Catalogo = new List<ExportColumn<FleteResponse>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("amount", "Importe", f => f.Amount),
            new("isDefault", "Por defecto", f => ExportBuilder.SiNo(f.IsDefault)),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "name", "amount", "isDefault", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportFletesQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportFletesQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Catalogos.Fletes.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t =>
                    t.Code.ToLower().Contains(search) ||
                    t.Name.ToLower().Contains(search));
            }

            if (request.IsDefault.HasValue)
            {
                query = query.Where(t => t.IsDefault == request.IsDefault.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(t => new FleteResponse
            {
                Code = t.Code,
                Name = t.Name,
                Amount = t.Amount,
                IsDefault = t.IsDefault,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedBy = t.UpdatedBy,
                RowVersion = t.RowVersion
            });

            var filas = await proyeccion
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Fletes", "fletes", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    request.IsDefault.HasValue ? $"Por defecto: {ExportBuilder.SiNo(request.IsDefault.Value)}" : null));
        }
    }
}
