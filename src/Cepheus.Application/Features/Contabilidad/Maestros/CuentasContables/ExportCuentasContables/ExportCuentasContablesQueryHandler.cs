using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.Common;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.CreateCuentaContable;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.ExportCuentasContables
{
    public class ExportCuentasContablesQueryHandler : IRequestHandler<ExportCuentasContablesQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<CuentaContableResponse>> Catalogo = new List<ExportColumn<CuentaContableResponse>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("tipoCuentaCode", "Tipo de cuenta", f => f.TipoCuentaCode),
            new("tipoAnexoCode", "Tipo de anexo", f => f.TipoAnexoCode),
            new("linkAccount1Code", "Cuenta de enlace 1", f => f.LinkAccount1Code),
            new("linkAccount2Code", "Cuenta de enlace 2", f => f.LinkAccount2Code),
            new("currencyCode", "Moneda", f => f.CurrencyCode),
            new("acmFlag", "Indicador ACM", f => f.AcmFlag),
            new("pilotFlag", "Indicador piloto", f => f.PilotFlag),
            new("gainsLossesFlag", "Ganancias y pérdidas", f => f.GainsLossesFlag),
            new("closingAccountCode", "Cuenta de cierre", f => f.ClosingAccountCode),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "name", "tipoCuentaCode", "tipoAnexoCode", "linkAccount1Code", "linkAccount2Code", "currencyCode", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportCuentasContablesQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportCuentasContablesQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Contabilidad.Maestros.CuentasContables.Query().AsNoTracking();

            // Mismos parámetros que sp_con_mira_cuentas (@digitos, @codigo, @nombre). A
            // diferencia del legacy, que usaba solo uno de ellos, aquí se combinan todos.
            if (request.Digits is > 0)
            {
                var digits = request.Digits.Value;
                query = query.Where(x => x.Code.Length == digits);
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var code = request.Code.Trim();
                query = query.Where(x => x.Code.StartsWith(code));
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var name = request.Name.Trim().ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(name));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            //var proyeccion = sortedQuery.Select(CreateCuentaContableCommandHandler.Map);

            //var filas = await proyeccion
            //    .Take(ExportBuilder.MaxFilas(formato) + 1)
            //    .ToListAsync(cancellationToken);
            var entidades = await sortedQuery
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            var filas = entidades
                .Select(CreateCuentaContableCommandHandler.Map)
                .ToList();

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Cuentas contables", "cuentas_contables", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(null, request.IsActive,
                    request.Digits.HasValue ? $"Dígitos: {request.Digits.Value}" : null,
                    string.IsNullOrWhiteSpace(request.Code) ? null : $"Código: {request.Code.Trim()}",
                    string.IsNullOrWhiteSpace(request.Name) ? null : $"Nombre: {request.Name.Trim()}"));
        }
    }
}
