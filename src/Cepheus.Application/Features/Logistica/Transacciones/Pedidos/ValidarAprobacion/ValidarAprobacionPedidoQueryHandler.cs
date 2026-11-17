using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.ValidarAprobacion
{
    public class ValidarAprobacionPedidoQueryHandler : IRequestHandler<ValidarAprobacionPedidoQuery, ValidarAprobacionPedidoResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly IAprobacionService _aprobacion;

        public ValidarAprobacionPedidoQueryHandler(IUnitOfWork uow, IAprobacionService aprobacion)
        {
            _uow = uow;
            _aprobacion = aprobacion;
        }

        public async Task<ValidarAprobacionPedidoResponse> Handle(ValidarAprobacionPedidoQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var pedido = await _uow.Logistica.Transacciones.Pedidos.Query()
                .AsNoTracking()
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.PlantaCode == plantaCode && p.Code == code, cancellationToken)
                ?? throw new KeyNotFoundException($"Pedido {plantaCode}/{code} no encontrado.");

            switch (pedido.EstadoPedido)
            {
                case EstadoPedido.Anulado:
                    throw new InvalidOperationException("La Orden de Requisición ya se encuentra anulada.");
                case EstadoPedido.EnCompra:
                case EstadoPedido.Comprado:
                    throw new InvalidOperationException("La Orden de Requisición ya se hizo la compra, no se puede aprobar.");
                case EstadoPedido.CompraParcial:
                case EstadoPedido.EntregaParcial:
                case EstadoPedido.Cerrado:
                    throw new InvalidOperationException("La Orden de Requisición ya tiene compras o entregas, no se puede aprobar.");
            }

            if (pedido.EstadoPedido == EstadoPedido.Aprobado)
            {
                // Legacy: aprobado -> solo quien lo aprobó puede ponerlo otra vez en Pendiente.
                var identificado = await _aprobacion.VerificarCredencialesAsync(request.Login, request.Clave, cancellationToken);

                if (!string.Equals(pedido.AprobadoPor, identificado.Login, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "La Orden de Requisición se encuentra aprobada. Solo quien la aprobó puede devolverla a Pendiente.");
                }

                return new ValidarAprobacionPedidoResponse(
                    "Desaprobar", identificado.Login, identificado.NombreCompleto, identificado.TrabajadorCode,
                    null, null, pedido.TotalPedido, AprobacionTipos.MonedaSoles,
                    $"La Orden de Requisición {pedido.Code} está aprobada. ¿Desea ponerla en Pendiente?");
            }

            if (pedido.Detalles.Count == 0)
            {
                throw new InvalidOperationException("La Orden de Requisición no tiene detalle. Verifique.");
            }

            var aprobador = await _aprobacion.ValidarAprobadorAsync(
                new DocumentoAprobable(
                    AprobacionTipos.Pedido, pedido.PlantaCode, pedido.Code, pedido.UnidadNegocioCode,
                    AprobacionTipos.MonedaSoles, pedido.TotalPedido),
                request.Login, request.Clave, cancellationToken);

            return new ValidarAprobacionPedidoResponse(
                "Aprobar", aprobador.Login, aprobador.NombreCompleto, aprobador.TrabajadorCode,
                aprobador.NivelCode, aprobador.Rol, pedido.TotalPedido, AprobacionTipos.MonedaSoles,
                $"¿Desea aprobar la Orden de Requisición {pedido.Code} de la planta {pedido.PlantaCode}?");
        }
    }
}
