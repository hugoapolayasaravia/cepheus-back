using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Administracion.Users.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.ExportUsers
{
    public class ExportUsersQueryHandler : IRequestHandler<ExportUsersQuery, ExportFileResponse>
    {
        private const int MaxFilasExcel = 50_000;
        private const int MaxFilasPdf = 5_000;

        // Columnas exportables: clave (la misma del listado de Angular) -> encabezado y valor.
        // Lista cerrada: cualquier clave que no esté aquí se ignora.
        private static readonly Dictionary<string, (string Encabezado, Func<UserResponse, object?> Valor)> Columnas =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["email"] = ("Correo", u => u.Email),
                ["firstName"] = ("Nombre", u => u.FirstName),
                ["lastName"] = ("Apellido", u => u.LastName),
                ["roles"] = ("Roles", u => string.Join(", ", u.Roles)),
                ["isActive"] = ("Estado", u => u.IsActive ? "Activo" : "Inactivo"),
                ["trabajadorCode"] = ("Cód. trabajador", u => u.TrabajadorCode),
                ["createdAt"] = ("Creado", u => HoraLocal.Convertir(u.CreatedAt)),
                ["createdBy"] = ("Creado por", u => u.CreatedBy),
                ["updatedAt"] = ("Modificado", u => HoraLocal.Convertir(u.UpdatedAt)),
                ["updatedBy"] = ("Modificado por", u => u.UpdatedBy)
            };

        private static readonly string[] ColumnasPorDefecto = { "email", "firstName", "lastName", "roles", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportUsersQueryHandler(
            IUnitOfWork uow,
            IExportService exportService,
            IReportSettings reportSettings,
            ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportUsersQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var maxFilas = formato == ExportFormat.Pdf ? MaxFilasPdf : MaxFilasExcel;

            var query = _uow.Administracion.Users.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().Contains(search) ||
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            // Se pide una fila de más para detectar si se supera el máximo.
            var usuarios = await sortedQuery
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    TrabajadorCode = u.TrabajadorCode,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    CreatedBy = u.CreatedBy,
                    UpdatedBy = u.UpdatedBy,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .Take(maxFilas + 1)
                .ToListAsync(cancellationToken);

            if (usuarios.Count > maxFilas)
            {
                throw new InvalidOperationException(
                    $"La exportación supera el máximo de {maxFilas:N0} filas para este formato. Aplica un filtro e inténtalo de nuevo.");
            }

            var claves = (request.Columns ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(c => Columnas.ContainsKey(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (claves.Count == 0)
            {
                claves = ColumnasPorDefecto.ToList();
            }

            var encabezados = claves.Select(c => Columnas[c].Encabezado).ToList();
            var filas = usuarios
                .Select(u => claves.Select(c => Columnas[c].Valor(u)).ToArray())
                .ToList();

            var ahora = HoraLocal.Ahora();

            if (formato == ExportFormat.Pdf)
            {
                var filtros = new List<string>();
                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    filtros.Add($"Búsqueda: {request.Search.Trim()}");
                }
                if (request.IsActive.HasValue)
                {
                    filtros.Add(request.IsActive.Value ? "Estado: Activos" : "Estado: Inactivos");
                }

                var subtitulo = (filtros.Count > 0 ? string.Join(" · ", filtros) + " · " : string.Empty)
                    + $"{filas.Count} registro(s)";

                var generadoPor = _currentUser.FullName ?? _currentUser.Email ?? "—";
                var info = new ReporteInfo(_reportSettings.CompanyName, "Usuarios", subtitulo, generadoPor, ahora);
                var pdf = _exportService.ToPdf(info, encabezados, filas);
                return new ExportFileResponse(pdf, "application/pdf", $"usuarios_{ahora:yyyyMMdd_HHmm}.pdf");
            }

            var excel = _exportService.ToExcel("Usuarios", encabezados, filas);
            return new ExportFileResponse(
                excel,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"usuarios_{ahora:yyyyMMdd_HHmm}.xlsx");
        }
    }
}
