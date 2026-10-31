using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.ExportResponsablesObra
{
    public class ExportResponsablesObraQueryHandler : IRequestHandler<ExportResponsablesObraQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<ResponsableObraResponse>> Catalogo = new List<ExportColumn<ResponsableObraResponse>>
        {
            new("clienteCode", "Cliente", f => f.ClienteCode),
            new("obraCode", "Obra", f => f.ObraCode),
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("phone", "Teléfono", f => f.Phone),
            new("position", "Cargo", f => f.Position),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "clienteCode", "obraCode", "code", "name", "phone", "position" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportResponsablesObraQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportResponsablesObraQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Catalogos.ResponsablesObra.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.ClienteCode))
            {
                query = query.Where(r => r.ClienteCode == request.ClienteCode);
            }

            if (!string.IsNullOrWhiteSpace(request.ObraCode))
            {
                query = query.Where(r => r.ObraCode == request.ObraCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(r =>
                    r.Name.Contains(search) ||
                    r.Position.Contains(search) ||
                    r.Phone.Contains(search));
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(r => r.ClienteCode).ThenBy(r => r.ObraCode).ThenBy(r => r.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(r => new ResponsableObraResponse
            {
                ClienteCode = r.ClienteCode,
                ObraCode = r.ObraCode,
                Code = r.Code,
                Name = r.Name,
                Phone = r.Phone,
                Position = r.Position,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                CreatedBy = r.CreatedBy,
                UpdatedBy = r.UpdatedBy,
                RowVersion = r.RowVersion
            });

            var filas = await proyeccion
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Responsables de obra", "responsables_obra", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, null,
                    string.IsNullOrWhiteSpace(request.ClienteCode) ? null : $"Cliente: {request.ClienteCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.ObraCode) ? null : $"Obra: {request.ObraCode.Trim()}"));
        }
    }
}
