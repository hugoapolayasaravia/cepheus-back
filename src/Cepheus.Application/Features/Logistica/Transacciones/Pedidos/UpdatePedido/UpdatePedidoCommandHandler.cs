// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/UpdatePedido/UpdatePedidoCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.CreatePedido;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.UpdatePedido
{
    /// <summary>
    /// Supuesto de negocio (no confirmado explícitamente, a validar): la
    /// cabecera del Pedido solo se puede editar mientras Estado ==
    /// Pendiente — mismo criterio de "bloqueo de campos por estado" que
    /// UpdateStockEntryCommandHandler en Logística. Si ya se aprobó, se
    /// gestiona vía ChangeEstadoPedidoCommand, no por aquí.
    /// </summary>
    public class UpdatePedidoCommandHandler : IRequestHandler<UpdatePedidoCommand, PedidoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePedidoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PedidoResponse> Handle(UpdatePedidoCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var pedido = await _uow.Logistica.Transacciones.Pedidos.Query()
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.PlantaCode == plantaCode && p.Code == code, cancellationToken);

            if (pedido is null)
            {
                throw new KeyNotFoundException($"Pedido {plantaCode}/{code} no encontrado.");
            }

            if (pedido.EstadoPedido != EstadoPedido.Pendiente)
            {
                throw new InvalidOperationException(
                    $"El Pedido está en estado '{pedido.EstadoPedido}' y ya no admite edición de cabecera.");
            }

            pedido.TipoPedidoCode = request.TipoPedidoCode.Trim().ToUpperInvariant();
            pedido.TipoValeCode = request.TipoValeCode.Trim().ToUpperInvariant();
            pedido.TramiteCode = request.TramiteCode.Trim().ToUpperInvariant();
            pedido.SubCentroCostoCode = request.SubCentroCostoCode.Trim().ToUpperInvariant();
            pedido.TrabajadorCode = request.TrabajadorCode.Trim().ToUpperInvariant();
            pedido.OrdenTrabajoCode = string.IsNullOrWhiteSpace(request.OrdenTrabajoCode) ? null : request.OrdenTrabajoCode.Trim().ToUpperInvariant();
            pedido.UnidadNegocioCode = request.UnidadNegocioCode.Trim().ToUpperInvariant();
            pedido.FechaEntrega = request.FechaEntrega;
            pedido.Observaciones = request.Observaciones?.Trim() ?? string.Empty;

            pedido.RowVersion = request.RowVersion;
            //_uow.Logistica.Transacciones.Pedidos.Update(pedido);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El Pedido fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreatePedidoCommandHandler.Map(pedido);
        }
    }
}