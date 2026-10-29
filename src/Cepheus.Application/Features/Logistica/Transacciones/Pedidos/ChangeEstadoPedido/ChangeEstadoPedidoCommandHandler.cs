// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/ChangeEstadoPedido/ChangeEstadoPedidoCommandHandler.cs
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.CreatePedido;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.ChangeEstadoPedido
{
    public class ChangeEstadoPedidoCommandHandler : IRequestHandler<ChangeEstadoPedidoCommand, PedidoResponse>
    {
        private static readonly Dictionary<EstadoPedido, EstadoPedido[]> ValidTransitions = new()
        {
            [EstadoPedido.Pendiente] = new[] { EstadoPedido.AprobacionProvisional, EstadoPedido.Aprobado, EstadoPedido.Anulado },
            [EstadoPedido.AprobacionProvisional] = new[] { EstadoPedido.Aprobado, EstadoPedido.Anulado },
            [EstadoPedido.Aprobado] = new[] { EstadoPedido.EnCompra, EstadoPedido.Anulado },
            [EstadoPedido.EnCompra] = new[] { EstadoPedido.CompraParcial, EstadoPedido.Comprado, EstadoPedido.Anulado },
            [EstadoPedido.CompraParcial] = new[] { EstadoPedido.Comprado, EstadoPedido.EntregaParcial, EstadoPedido.Anulado },
            [EstadoPedido.Comprado] = Array.Empty<EstadoPedido>(),
            [EstadoPedido.EntregaParcial] = new[] { EstadoPedido.Cerrado },
            [EstadoPedido.Cerrado] = Array.Empty<EstadoPedido>(),
            [EstadoPedido.Anulado] = Array.Empty<EstadoPedido>()
        };

        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public ChangeEstadoPedidoCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<PedidoResponse> Handle(ChangeEstadoPedidoCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            if (!System.Enum.TryParse<EstadoPedido>(request.NuevoEstado, true, out var nuevoEstado))
            {
                throw new ArgumentException($"Estado '{request.NuevoEstado}' no es válido.");
            }

            var pedido = await _uow.Logistica.Transacciones.Pedidos.Query()
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.PlantaCode == plantaCode && p.Code == code, cancellationToken);

            if (pedido is null)
            {
                throw new KeyNotFoundException($"Pedido {plantaCode}/{code} no encontrado.");
            }

            if (!ValidTransitions[pedido.EstadoPedido].Contains(nuevoEstado))
            {
                throw new InvalidOperationException(
                    $"No se puede pasar de '{pedido.EstadoPedido}' a '{nuevoEstado}'. " +
                    $"Transiciones válidas desde '{pedido.EstadoPedido}': " +
                    $"{string.Join(", ", ValidTransitions[pedido.EstadoPedido])}.");
            }

            pedido.EstadoPedido = nuevoEstado;

            if (nuevoEstado == EstadoPedido.Aprobado)
            {
                pedido.AprobadoPor = _currentUser.FullName;
                pedido.FechaAprobacion = DateTime.UtcNow;
            }
            else if (nuevoEstado == EstadoPedido.EnCompra)
            {
                pedido.CompradoPor = _currentUser.FullName;
                pedido.FechaCompra = DateTime.UtcNow;
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return CreatePedidoCommandHandler.Map(pedido);
        }
    }
}