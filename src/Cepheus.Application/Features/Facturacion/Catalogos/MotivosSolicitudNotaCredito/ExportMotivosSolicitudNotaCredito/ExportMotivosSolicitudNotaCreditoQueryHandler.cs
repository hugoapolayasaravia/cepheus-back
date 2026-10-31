using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.ExportMotivosSolicitudNotaCredito
{
    public class ExportMotivosSolicitudNotaCreditoQueryHandler : IRequestHandler<ExportMotivosSolicitudNotaCreditoQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<MotivoSolicitudNotaCreditoResponse>> Catalogo = new List<ExportColumn<MotivoSolicitudNotaCreditoResponse>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("parentCode", "Motivo padre", f => f.ParentCode),
            new("parentName", "Nombre del motivo padre", f => f.ParentName),
            new("motivoDevolucionCode", "Motivo de devolución", f => f.MotivoDevolucionCode),
            new("motivoDevolucionName", "Nombre del motivo de devolución", f => f.MotivoDevolucionName),
            new("affectsStock", "Afecta stock", f => ExportBuilder.SiNo(f.AffectsStock)),
            new("startDate", "Inicio", f => f.StartDate?.ToString("dd/MM/yyyy")),
            new("endDate", "Fin", f => f.EndDate?.ToString("dd/MM/yyyy")),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "name", "parentCode", "parentName", "motivoDevolucionCode", "motivoDevolucionName", "affectsStock", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportMotivosSolicitudNotaCreditoQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportMotivosSolicitudNotaCreditoQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t =>
                    t.Code.ToLower().Contains(search) ||
                    t.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.ParentCode))
            {
                var parent = request.ParentCode.Trim();
                query = query.Where(t => t.ParentCode == parent);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            if (request.OnlyCurrent == true)
            {
                var today = DateTime.UtcNow.Date;
                query = query.Where(t =>
                    (t.StartDate == null || t.StartDate <= today) &&
                    (t.EndDate == null || t.EndDate >= today));
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(MotivoSolicitudNotaCreditoProjection.ToResponse);

            var filas = await proyeccion
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Motivos de solicitud de nota de crédito", "motivos_solicitud_nota_credito", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    string.IsNullOrWhiteSpace(request.ParentCode) ? null : $"Motivo padre: {request.ParentCode.Trim()}",
                    request.OnlyCurrent.HasValue ? $"Solo vigentes: {ExportBuilder.SiNo(request.OnlyCurrent.Value)}" : null));
        }
    }
}
