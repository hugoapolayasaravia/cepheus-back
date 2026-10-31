using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.ExportFormasPagoVenta
{
    public class ExportFormasPagoVentaQueryHandler : IRequestHandler<ExportFormasPagoVentaQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<FormaPagoVentaResponse>> Catalogo = new List<ExportColumn<FormaPagoVentaResponse>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("days", "Días", f => f.Days),
            new("isCredit", "Crédito", f => ExportBuilder.SiNo(f.IsCredit)),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "name", "days", "isCredit", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportFormasPagoVentaQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportFormasPagoVentaQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Catalogos.FormasPagoVenta.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t =>
                    t.Code.ToLower().Contains(search) ||
                    t.Name.ToLower().Contains(search));
            }

            if (request.IsCredit.HasValue)
            {
                query = query.Where(t => t.IsCredit == request.IsCredit.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(t => new FormaPagoVentaResponse
            {
                Code = t.Code,
                Name = t.Name,
                Days = t.Days,
                IsCredit = t.IsCredit,
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
                "Formas de pago de venta", "formas_pago_venta", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    request.IsCredit.HasValue ? $"Crédito: {ExportBuilder.SiNo(request.IsCredit.Value)}" : null));
        }
    }
}
