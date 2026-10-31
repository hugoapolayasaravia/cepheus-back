using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.Common;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.CreateCuentaBancaria;
using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.CreateVendedor;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.ExportCuentasBancarias
{
    public class ExportCuentasBancariasQueryHandler : IRequestHandler<ExportCuentasBancariasQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<CuentaBancariaResponse>> Catalogo = new List<ExportColumn<CuentaBancariaResponse>>
        {
            new("bankCode", "Banco", f => f.BankCode),
            new("accountNumber", "N.º de cuenta", f => f.AccountNumber),
            new("currencyCode", "Moneda", f => f.CurrencyCode),
            new("checkStart", "Cheque inicial", f => f.CheckStart),
            new("checkEnd", "Cheque final", f => f.CheckEnd),
            new("checkCurrent", "Cheque actual", f => f.CheckCurrent),
            new("controlFlag", "Indicador de control", f => f.ControlFlag),
            new("accountCode", "Cuenta contable", f => f.AccountCode),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "bankCode", "accountNumber", "currencyCode", "checkStart", "checkEnd", "checkCurrent", "controlFlag", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportCuentasBancariasQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportCuentasBancariasQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Contabilidad.Maestros.CuentasBancarias.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.AccountNumber.ToLower().Contains(search) ||
                    x.BankCode.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.BankCode))
            {
                var bank = request.BankCode.Trim();
                query = query.Where(x => x.BankCode == bank);
            }

            if (!string.IsNullOrWhiteSpace(request.CurrencyCode))
            {
                var currency = request.CurrencyCode.Trim().ToUpper();
                query = query.Where(x => x.CurrencyCode == currency);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.BankCode).ThenBy(x => x.AccountNumber)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            //var proyeccion = sortedQuery.Select(CreateCuentaBancariaCommandHandler.Map);

            //var filas = await proyeccion
            //    .Take(ExportBuilder.MaxFilas(formato) + 1)
            //    .ToListAsync(cancellationToken);
            var entidades = await sortedQuery
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            var filas = entidades
                .Select(CreateCuentaBancariaCommandHandler.Map)
                .ToList();

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Cuentas bancarias", "cuentas_bancarias", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    string.IsNullOrWhiteSpace(request.BankCode) ? null : $"Banco: {request.BankCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.CurrencyCode) ? null : $"Moneda: {request.CurrencyCode.Trim()}"));
        }
    }
}
