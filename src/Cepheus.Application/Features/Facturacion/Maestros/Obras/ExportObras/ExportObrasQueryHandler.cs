using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Maestros.Obras.Common;
using Cepheus.Application.Features.Facturacion.Maestros.Obras.CreateObra;
using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.CreateVendedor;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Obras.ExportObras
{
    public class ExportObrasQueryHandler : IRequestHandler<ExportObrasQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<ObraResponse>> Catalogo = new List<ExportColumn<ObraResponse>>
        {
            new("clienteCode", "Cliente", f => f.ClienteCode),
            new("code", "Código", f => f.Code),
            new("description", "Descripción", f => f.Description),
            new("address", "Dirección", f => f.Address),
            new("ubigeoCode", "Ubigeo", f => f.UbigeoCode),
            new("observations", "Observaciones", f => f.Observations),
            new("estado", "Estado", f => f.Estado),
            new("deliveryAddress", "Dirección de entrega", f => f.DeliveryAddress),
            new("deliveryUbigeoCode", "Ubigeo de entrega", f => f.DeliveryUbigeoCode),
            new("billingAddress", "Dirección de facturación", f => f.BillingAddress),
            new("billingUbigeoCode", "Ubigeo de facturación", f => f.BillingUbigeoCode),
            new("responsibleName", "Responsable", f => f.ResponsibleName),
            new("responsiblePhone", "Teléfono del responsable", f => f.ResponsiblePhone),
            new("responsibleEmail", "Correo del responsable", f => f.ResponsibleEmail),
            new("formaPagoVentaCode", "Forma de pago", f => f.FormaPagoVentaCode),
            new("cobradorCode", "Cobrador", f => f.CobradorCode),
            new("vendedorCode", "Vendedor", f => f.VendedorCode),
            new("analisisVentaCode", "Análisis de venta", f => f.AnalisisVentaCode),
            new("creditLimit", "Límite de crédito", f => f.CreditLimit),
            new("creditCurrencyCode", "Moneda del crédito", f => f.CreditCurrencyCode),
            new("entryDate", "Fecha de ingreso", f => f.EntryDate?.ToString("dd/MM/yyyy")),
            new("hasSurcharge", "Con recargo", f => ExportBuilder.SiNo(f.HasSurcharge)),
            new("shortName", "Nombre corto", f => f.ShortName),
            new("tipoValorizacionCode", "Tipo de valorización", f => f.TipoValorizacionCode),
            new("requiresValorizacion", "Requiere valorización", f => f.RequiresValorizacion.HasValue ? ExportBuilder.SiNo(f.RequiresValorizacion.Value) : null),
            new("scheduledWeekday", "Día de programación", f => f.ScheduledWeekday),
            new("isProject", "Es proyecto", f => ExportBuilder.SiNo(f.IsProject)),
            new("requiresPrinting", "Requiere impresión", f => ExportBuilder.SiNo(f.RequiresPrinting)),
            new("completionDate", "Fecha de término", f => f.CompletionDate?.ToString("dd/MM/yyyy")),
            new("completionUser", "Usuario de término", f => f.CompletionUser),
            new("requiresManagementApproval", "Requiere aprobación de gerencia", f => f.RequiresManagementApproval.HasValue ? ExportBuilder.SiNo(f.RequiresManagementApproval.Value) : null),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "clienteCode", "code", "description", "address", "ubigeoCode", "observations", "deliveryAddress", "estado" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportObrasQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportObrasQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Maestros.Obras.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(o =>
                    o.Code.ToLower().Contains(search) ||
                    o.Description.ToLower().Contains(search) ||
                    o.ShortName.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.ClienteCode))
            {
                var clienteCode = request.ClienteCode.Trim().ToUpper();
                query = query.Where(o => o.ClienteCode == clienteCode);
            }

            if (!string.IsNullOrWhiteSpace(request.VendedorCode))
            {
                var vendedorCode = request.VendedorCode.Trim().ToUpper();
                query = query.Where(o => o.VendedorCode == vendedorCode);
            }

            if (!string.IsNullOrWhiteSpace(request.CobradorCode))
            {
                var cobradorCode = request.CobradorCode.Trim().ToUpper();
                query = query.Where(o => o.CobradorCode == cobradorCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Estado) &&
                System.Enum.TryParse<EstadoObra>(request.Estado, true, out var estado))
            {
                query = query.Where(o => o.Estado == estado);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(o => o.ClienteCode).ThenBy(o => o.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            //var proyeccion = sortedQuery.Select(Cepheus.Application.Features.Facturacion.Maestros.Obras.CreateObra.CreateObraCommandHandler.Map);

            //var filas = await proyeccion
            //    .Take(ExportBuilder.MaxFilas(formato) + 1)
            //    .ToListAsync(cancellationToken);
            var entidades = await sortedQuery
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            var filas = entidades
                .Select(CreateObraCommandHandler.Map)
                .ToList();

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Obras", "obras", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, null,
                    string.IsNullOrWhiteSpace(request.ClienteCode) ? null : $"Cliente: {request.ClienteCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.VendedorCode) ? null : $"Vendedor: {request.VendedorCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.CobradorCode) ? null : $"Cobrador: {request.CobradorCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.Estado) ? null : $"Estado: {request.Estado.Trim()}"));
        }
    }
}
