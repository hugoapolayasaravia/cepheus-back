using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.CreatePedido;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.ExportPedidos
{
    public class ExportPedidosQueryHandler : IRequestHandler<ExportPedidosQuery, ExportFileResponse>
    {
        private static string TextoEstado(string estado) => estado switch
        {
            nameof(EstadoPedido.Pendiente) => "Pendiente",
            nameof(EstadoPedido.AprobacionProvisional) => "Aprobación provisional",
            nameof(EstadoPedido.Aprobado) => "Aprobado",
            nameof(EstadoPedido.EnCompra) => "En compra",
            nameof(EstadoPedido.CompraParcial) => "Compra parcial",
            nameof(EstadoPedido.Comprado) => "Comprado",
            nameof(EstadoPedido.EntregaParcial) => "Entrega parcial",
            nameof(EstadoPedido.Cerrado) => "Cerrado",
            nameof(EstadoPedido.Anulado) => "Anulado",
            _ => estado
        };

        private static readonly IReadOnlyList<ExportColumn<PedidoResponse>> Catalogo = new List<ExportColumn<PedidoResponse>>
        {
            new("plantaCode", "Planta", p => p.PlantaCode),
            new("code", "Pedido", p => p.Code),
            new("estadoPedido", "Estado", p => TextoEstado(p.EstadoPedido)),
            new("tipoPedidoCode", "Tipo", p => p.TipoPedidoCode),
            new("tramiteCode", "Trámite", p => p.TramiteCode),
            new("subCentroCostoCode", "Subcentro de costo", p => p.SubCentroCostoCode),
            new("trabajadorCode", "Trabajador", p => p.TrabajadorCode),
            new("ordenTrabajoCode", "O.T.", p => p.OrdenTrabajoCode),
            new("unidadNegocioCode", "Unidad de negocio", p => p.UnidadNegocioCode),
            new("createdAt", "Fecha proceso", p => HoraLocal.Convertir(p.CreatedAt)),
            new("fechaEntrega", "Fecha entrega", p => p.FechaEntrega.ToString("dd/MM/yyyy")),
            new("lineas", "Líneas", p => p.Detalles.Count),
            new("netoPedido", "Neto", p => p.NetoPedido),
            new("igvPedido", "IGV", p => p.IgvPedido),
            new("totalPedido", "Total", p => p.TotalPedido),
            new("observaciones", "Observaciones", p => p.Observaciones),
            new("createdBy", "Creado por", p => p.CreatedBy),
            new("aprobadoPor", "Aprobado por", p => p.AprobadoPor),
            new("fechaAprobacion", "Fecha aprobación", p => HoraLocal.Convertir(p.FechaAprobacion)),
        };

        private static readonly string[] PorDefecto =
        {
            "plantaCode", "code", "estadoPedido", "tipoPedidoCode", "subCentroCostoCode",
            "createdAt", "fechaEntrega", "totalPedido", "createdBy"
        };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportPedidosQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportPedidosQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Logistica.Transacciones.Pedidos.Query()
                .AsNoTracking()
                .Include(p => p.Detalles)
                .AsQueryable();

            // Igual que el listado: solo las plantas en las que el usuario está inscrito.
            var uid = _currentUser.UserId ?? -1;
            var misPlantas = _uow.Logistica.Catalogos.UsuariosPlanta.Query()
                .Where(x => x.UserId == uid).Select(x => x.PlantaCode);
            query = query.Where(p => misPlantas.Contains(p.PlantaCode));

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(p => p.Code.ToLower().Contains(search) || p.Observaciones.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var plantaCode = request.PlantaCode.Trim().ToUpper();
                query = query.Where(p => p.PlantaCode == plantaCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Estado)
                && System.Enum.TryParse<EstadoPedido>(request.Estado, true, out var estado))
            {
                query = query.Where(p => p.EstadoPedido == estado);
            }

            if (!string.IsNullOrWhiteSpace(request.TrabajadorCode))
            {
                var trabajadorCode = request.TrabajadorCode.Trim().ToUpper();
                query = query.Where(p => p.TrabajadorCode == trabajadorCode);
            }

            if (!string.IsNullOrWhiteSpace(request.SubCentroCostoCode))
            {
                var subCentroCostoCode = request.SubCentroCostoCode.Trim().ToUpper();
                query = query.Where(p => p.SubCentroCostoCode == subCentroCostoCode);
            }

            if (!string.IsNullOrWhiteSpace(request.TipoPedidoCode))
            {
                var tipoPedidoCode = request.TipoPedidoCode.Trim().ToUpper();
                query = query.Where(p => p.TipoPedidoCode == tipoPedidoCode);
            }

            if (!string.IsNullOrWhiteSpace(request.CreatedBy))
            {
                var createdBy = request.CreatedBy.Trim().ToLower();
                query = query.Where(p => p.CreatedBy != null && p.CreatedBy.ToLower().Contains(createdBy));
            }

            if (request.FechaProcesoDesde.HasValue)
            {
                var desde = request.FechaProcesoDesde.Value.Date;
                query = query.Where(p => p.CreatedAt >= desde);
            }

            if (request.FechaProcesoHasta.HasValue)
            {
                var hasta = request.FechaProcesoHasta.Value.Date.AddDays(1);
                query = query.Where(p => p.CreatedAt < hasta);
            }

            if (request.FechaEntregaDesde.HasValue)
            {
                query = query.Where(p => p.FechaEntrega >= request.FechaEntregaDesde.Value);
            }

            if (request.FechaEntregaHasta.HasValue)
            {
                query = query.Where(p => p.FechaEntrega <= request.FechaEntregaHasta.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.ApplySort(request.SortBy, request.SortDesc ?? true);

            var entidades = await sorted
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            var filas = entidades.Select(CreatePedidoCommandHandler.Map).ToList();

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Pedidos", "pedidos", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, null,
                    !string.IsNullOrWhiteSpace(request.PlantaCode) ? $"Planta: {request.PlantaCode}" : null,
                    !string.IsNullOrWhiteSpace(request.Estado) ? $"Estado: {TextoEstado(request.Estado.Trim())}" : null,
                    !string.IsNullOrWhiteSpace(request.TipoPedidoCode) ? $"Tipo: {request.TipoPedidoCode}" : null,
                    !string.IsNullOrWhiteSpace(request.SubCentroCostoCode) ? $"Subcentro: {request.SubCentroCostoCode}" : null,
                    request.FechaProcesoDesde.HasValue ? $"Desde: {request.FechaProcesoDesde:dd/MM/yyyy}" : null,
                    request.FechaProcesoHasta.HasValue ? $"Hasta: {request.FechaProcesoHasta:dd/MM/yyyy}" : null));
        }
    }
}
