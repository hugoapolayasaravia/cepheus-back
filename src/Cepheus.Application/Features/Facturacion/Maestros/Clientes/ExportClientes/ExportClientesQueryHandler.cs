using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.Common;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.CreateCliente;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Clientes.ExportClientes
{
    public class ExportClientesQueryHandler : IRequestHandler<ExportClientesQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<ClienteResponse>> Catalogo = new List<ExportColumn<ClienteResponse>>
        {
            new("code", "Código", f => f.Code),
            new("personType", "Tipo de persona", f => f.PersonType),
            new("documentTypeCode", "Tipo de documento", f => f.DocumentTypeCode),
            new("documentNumber", "N.º de documento", f => f.DocumentNumber),
            new("name", "Nombre", f => f.Name),
            new("address", "Dirección", f => f.Address),
            new("ubigeoCode", "Ubigeo", f => f.UbigeoCode),
            new("phone", "Teléfono", f => f.Phone),
            new("parentClientCode", "Cliente principal", f => f.ParentClientCode),
            new("tipoClienteCode", "Tipo de cliente", f => f.TipoClienteCode),
            new("clasificacionClienteCode", "Clasificación", f => f.ClasificacionClienteCode),
            new("legalRepresentativeName", "Representante legal", f => f.LegalRepresentativeName),
            new("legalRepresentativePhone", "Teléfono del representante", f => f.LegalRepresentativePhone),
            new("legalRepresentativeDni", "DNI del representante", f => f.LegalRepresentativeDni),
            new("contactName", "Contacto", f => f.ContactName),
            new("contactPhone", "Teléfono de contacto", f => f.ContactPhone),
            new("contactEmail", "Correo de contacto", f => f.ContactEmail),
            new("observations", "Observaciones", f => f.Observations),
            new("isVip", "VIP", f => ExportBuilder.SiNo(f.IsVip)),
            new("requiresCashOnly", "Solo contado", f => ExportBuilder.SiNo(f.RequiresCashOnly)),
            new("hasGlobalCreditLine", "Línea de crédito global", f => ExportBuilder.SiNo(f.HasGlobalCreditLine)),
            new("globalCreditAmount", "Importe de crédito global", f => f.GlobalCreditAmount),
            new("requiresPurchaseOrderApproval", "Requiere aprobación de OC", f => f.RequiresPurchaseOrderApproval.HasValue ? ExportBuilder.SiNo(f.RequiresPurchaseOrderApproval.Value) : null),
            new("requiresWorkOrderApproval", "Requiere aprobación de OT", f => f.RequiresWorkOrderApproval.HasValue ? ExportBuilder.SiNo(f.RequiresWorkOrderApproval.Value) : null),
            new("formaPagoVentaCode", "Forma de pago", f => f.FormaPagoVentaCode),
            new("currencyCode", "Moneda", f => f.CurrencyCode),
            new("requiresManagementApproval", "Requiere aprobación de gerencia", f => f.RequiresManagementApproval.HasValue ? ExportBuilder.SiNo(f.RequiresManagementApproval.Value) : null),
            new("estado", "Estado", f => f.Estado),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "personType", "documentTypeCode", "documentNumber", "name", "address", "ubigeoCode", "estado" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportClientesQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportClientesQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Maestros.Clientes.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(c =>
                    c.Code.ToLower().Contains(search) ||
                    c.DocumentNumber.ToLower().Contains(search) ||
                    (c.Name != null && c.Name.ToLower().Contains(search)) ||
                    c.ContactName.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.TipoClienteCode))
            {
                var tipoClienteCode = request.TipoClienteCode.Trim().ToUpper();
                query = query.Where(c => c.TipoClienteCode == tipoClienteCode);
            }

            if (!string.IsNullOrWhiteSpace(request.ClasificacionClienteCode))
            {
                var clasificacionCode = request.ClasificacionClienteCode.Trim().ToUpper();
                query = query.Where(c => c.ClasificacionClienteCode == clasificacionCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Estado) &&
                System.Enum.TryParse<EstadoCliente>(request.Estado, true, out var estado))
            {
                query = query.Where(c => c.Estado == estado);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(c => c.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            //var proyeccion = sortedQuery.Select(Cepheus.Application.Features.Facturacion.Maestros.Clientes.CreateCliente.CreateClienteCommandHandler.Map);

            //var filas = await proyeccion
            //    .Take(ExportBuilder.MaxFilas(formato) + 1)
            //    .ToListAsync(cancellationToken);
            var entidades = await sortedQuery
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            var filas = entidades
                .Select(CreateClienteCommandHandler.Map)
                .ToList();

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Clientes", "clientes", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, null,
                    string.IsNullOrWhiteSpace(request.TipoClienteCode) ? null : $"Tipo de cliente: {request.TipoClienteCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.ClasificacionClienteCode) ? null : $"Clasificación: {request.ClasificacionClienteCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.Estado) ? null : $"Estado: {request.Estado.Trim()}"));
        }
    }
}
