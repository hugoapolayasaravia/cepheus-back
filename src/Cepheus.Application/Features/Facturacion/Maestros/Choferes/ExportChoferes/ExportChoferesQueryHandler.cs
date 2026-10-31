using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Maestros.Choferes.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.ExportChoferes
{
    public class ExportChoferesQueryHandler : IRequestHandler<ExportChoferesQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<ChoferResponse>> Catalogo = new List<ExportColumn<ChoferResponse>>
        {
            new("transportistaCode", "Transportista", f => f.TransportistaCode),
            new("code", "Código", f => f.Code),
            new("fullName", "Nombre completo", f => f.FullName),
            new("driverLicenseNumber", "N.º de licencia", f => f.DriverLicenseNumber),
            new("observations", "Observaciones", f => f.Observations),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "transportistaCode", "code", "fullName", "driverLicenseNumber", "observations", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportChoferesQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportChoferesQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Maestros.Choferes.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(c =>
                    c.Code.ToLower().Contains(search) ||
                    c.FullName.ToLower().Contains(search) ||
                    c.DriverLicenseNumber.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.TransportistaCode))
            {
                var transportistaCode = request.TransportistaCode.Trim().ToUpper();
                query = query.Where(c => c.TransportistaCode == transportistaCode);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(c => c.TransportistaCode).ThenBy(c => c.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(c => new ChoferResponse
            {
                TransportistaCode = c.TransportistaCode,
                Code = c.Code,
                FullName = c.FullName,
                DriverLicenseNumber = c.DriverLicenseNumber,
                Observations = c.Observations,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy,
                RowVersion = c.RowVersion
            });

            var filas = await proyeccion
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Choferes", "choferes", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    string.IsNullOrWhiteSpace(request.TransportistaCode) ? null : $"Transportista: {request.TransportistaCode.Trim()}"));
        }
    }
}
