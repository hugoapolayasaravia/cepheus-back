// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/CreatePedido/CreatePedidoCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.CreatePedido
{
    public class CreatePedidoCommandHandler : IRequestHandler<CreatePedidoCommand, PedidoResponse>
    {
        private const int MaxConcurrencyRetries = 3;
        private const int CorrelativeLength = 5;
        private const string CodePrefix = "7";

        private readonly IUnitOfWork _uow;

        public CreatePedidoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PedidoResponse> Handle(CreatePedidoCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var nextCode = await NextCodeAsync(plantaCode, cancellationToken);

                var pedido = new Pedido
                {
                    PlantaCode = plantaCode,
                    Code = nextCode,
                    CodPlanta = plantaCode,

                    TipoPedidoCode = request.TipoPedidoCode.Trim().ToUpperInvariant(),
                    TipoValeCode = request.TipoValeCode,
                    TramiteCode = request.TramiteCode.Trim().ToUpperInvariant(),
                    SubCentroCostoCode = request.SubCentroCostoCode.Trim().ToUpperInvariant(),
                    TrabajadorCode = request.TrabajadorCode.Trim().ToUpperInvariant(),
                    OrdenTrabajoCode = Normalize(request.OrdenTrabajoCode),
                    UnidadNegocioCode = request.UnidadNegocioCode.Trim().ToUpperInvariant(),

                    FechaEntrega = request.FechaEntrega,
                    EstadoPedido = EstadoPedido.Pendiente,
                    Observaciones = request.Observaciones?.Trim() ?? string.Empty
                };

                var item = 1;
                foreach (var line in request.Detalles)
                {
                    pedido.Detalles.Add(new PedidoDetalle
                    {
                        PlantaCode = plantaCode,
                        PedidoCode = nextCode,
                        ItemNumber = item++,
                        ArticuloCode = Normalize(line.ArticuloCode),
                        DescripcionArticulo = line.DescripcionArticulo.Trim(),
                        UnidadMedidaCode = line.UnidadMedidaCode.Trim().ToUpperInvariant(),
                        PrecioArticulo = line.PrecioArticulo,
                        CantidadArticulo = line.CantidadArticulo,
                        TotalArticulo = PedidoTotalsCalculator.CalculateLineTotal(line.PrecioArticulo, line.CantidadArticulo),
                        EstadoPedidoDetalle = EstadoPedidoDetalle.Pendiente,
                        ProveedorCode = Normalize(line.ProveedorCode)
                    });
                }

                await PedidoTotalsCalculator.RecalculateAsync(_uow, pedido, cancellationToken);

                await _uow.Logistica.Transacciones.Pedidos.AddAsync(pedido, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                    return Map(pedido);
                }
                catch (DbUpdateException) when (attempt < MaxConcurrencyRetries)
                {
                    _uow.ClearTracking();
                }
            }

            throw new InvalidOperationException(
                $"No se pudo generar el correlativo de Pedido para la planta {plantaCode} por alta concurrencia. Intente nuevamente.");
        }

        private async Task<string> NextCodeAsync(string plantaCode, CancellationToken cancellationToken)
        {
            var lastCode = await _uow.Logistica.Transacciones.Pedidos.Query()
                .AsNoTracking()
                .Where(p => p.PlantaCode == plantaCode && p.Code.StartsWith(CodePrefix))
                .OrderByDescending(p => p.Code)
                .Select(p => p.Code)
                .FirstOrDefaultAsync(cancellationToken);

            var next = 1;
            if (lastCode is not null && int.TryParse(lastCode.Substring(CodePrefix.Length), out var lastNumber))
            {
                next = lastNumber + 1;
            }

            return CodePrefix + next.ToString().PadLeft(CorrelativeLength, '0');
        }

        private static string? Normalize(string? code)
            => string.IsNullOrWhiteSpace(code) ? null : code.Trim().ToUpperInvariant();

        internal static PedidoResponse Map(Pedido p) => new()
        {
            PlantaCode = p.PlantaCode,
            Code = p.Code,
            CodPlanta = p.CodPlanta,
            TipoPedidoCode = p.TipoPedidoCode,
            TipoValeCode = p.TipoValeCode,
            TramiteCode = p.TramiteCode,
            SubCentroCostoCode = p.SubCentroCostoCode,
            TrabajadorCode = p.TrabajadorCode,
            OrdenTrabajoCode = p.OrdenTrabajoCode,
            UnidadNegocioCode = p.UnidadNegocioCode,
            FechaEntrega = p.FechaEntrega,
            NetoPedido = p.NetoPedido,
            IgvPedido = p.IgvPedido,
            TotalPedido = p.TotalPedido,
            EstadoPedido = p.EstadoPedido.ToString(),
            Observaciones = p.Observaciones,
            AprobadoPor = p.AprobadoPor,
            FechaAprobacion = p.FechaAprobacion,
            CompradoPor = p.CompradoPor,
            FechaCompra = p.FechaCompra,
            Detalles = p.Detalles.Select(d => new PedidoDetalleResponse
            {
                PlantaCode = d.PlantaCode,
                PedidoCode = d.PedidoCode,
                ItemNumber = d.ItemNumber,
                ArticuloCode = d.ArticuloCode,
                DescripcionArticulo = d.DescripcionArticulo,
                UnidadMedidaCode = d.UnidadMedidaCode,
                PrecioArticulo = d.PrecioArticulo,
                CantidadArticulo = d.CantidadArticulo,
                TotalArticulo = d.TotalArticulo,
                EstadoPedidoDetalle = d.EstadoPedidoDetalle.ToString(),
                OrdenCompraCode = d.OrdenCompraCode,
                ProveedorCode = d.ProveedorCode,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
                RowVersion = d.RowVersion
            }).ToList(),
            CreatedAt = p.CreatedAt,
            CreatedBy = p.CreatedBy,
            UpdatedAt = p.UpdatedAt,
            RowVersion = p.RowVersion
        };
    }
}