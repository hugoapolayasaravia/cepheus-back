// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/ChangeEstadoOrdenCompra/ChangeEstadoOrdenCompraCommandHandler.cs
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.ChangeEstadoOrdenCompra
{
    public class ChangeEstadoOrdenCompraCommandHandler : IRequestHandler<ChangeEstadoOrdenCompraCommand, OrdenCompraResponse>
    {
        private static readonly Dictionary<EstadoOrdenCompra, EstadoOrdenCompra[]> ValidTransitions = new()
        {
            [EstadoOrdenCompra.Pendiente] = new[] { EstadoOrdenCompra.Aprobado, EstadoOrdenCompra.Anulado },
            [EstadoOrdenCompra.Aprobado] = new[] { EstadoOrdenCompra.Pendiente, EstadoOrdenCompra.EntregaParcial, EstadoOrdenCompra.Cerrado, EstadoOrdenCompra.Anulado },
            [EstadoOrdenCompra.EntregaParcial] = new[] { EstadoOrdenCompra.Cerrado },
            [EstadoOrdenCompra.Cerrado] = Array.Empty<EstadoOrdenCompra>(),
            [EstadoOrdenCompra.Anulado] = Array.Empty<EstadoOrdenCompra>()
        };

        private readonly IUnitOfWork _uow;
        private readonly IAprobacionService _aprobacion;
        private readonly ICurrentUserService _currentUser;

        public ChangeEstadoOrdenCompraCommandHandler(IUnitOfWork uow, IAprobacionService aprobacion, ICurrentUserService currentUser)
        {
            _uow = uow;
            _aprobacion = aprobacion;
            _currentUser = currentUser;
        }

        public async Task<OrdenCompraResponse> Handle(ChangeEstadoOrdenCompraCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            if (!System.Enum.TryParse<EstadoOrdenCompra>(request.NuevoEstado, true, out var nuevoEstado))
            {
                throw new ArgumentException($"Estado '{request.NuevoEstado}' no es válido.");
            }

            var orden = await _uow.Logistica.Transacciones.OrdenesCompra.Query()
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.PlantaCode == plantaCode && o.Code == code, cancellationToken);

            if (orden is null)
            {
                throw new KeyNotFoundException($"Orden de Compra {plantaCode}/{code} no encontrada.");
            }

            if (!ValidTransitions[orden.Estado].Contains(nuevoEstado))
            {
                throw new InvalidOperationException(
                    $"No se puede pasar de '{orden.Estado}' a '{nuevoEstado}'. " +
                    $"Transiciones válidas: {string.Join(", ", ValidTransitions[orden.Estado])}.");
            }

            if (nuevoEstado == EstadoOrdenCompra.Aprobado)
            {
                await ValidarAprobadorAsync(orden, cancellationToken);
                orden.AprobadoPor = _currentUser.Login;
                orden.FechaAprobacion = DateTime.Now;
            }

            if (orden.Estado == EstadoOrdenCompra.Aprobado && nuevoEstado == EstadoOrdenCompra.Pendiente)
            {
                // Legacy (w_log_aprobacion_tra): solo quien aprobó puede devolverla a Pendiente, y sin entregas.
                if (!string.Equals(orden.AprobadoPor, _currentUser.Login, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"Usuario es diferente al que aprobó la transacción. Solo el usuario {orden.AprobadoPor} puede volverla a Pendiente.");
                }

                if (orden.Detalles.Any(d => d.CantidadEntregada > 0))
                {
                    throw new InvalidOperationException("La Orden de Compra tiene entregas, no se puede volver a Pendiente.");
                }

                orden.AprobadoPor = null;
                orden.FechaAprobacion = null;
            }

            orden.Estado = nuevoEstado;

            await _uow.SaveChangesAsync(cancellationToken);

            return OrdenCompraMapper.Map(orden);
        }

        private Task ValidarAprobadorAsync(Domain.Logistica.Transacciones.OrdenCompra orden, CancellationToken cancellationToken)
        {
            // Regla única de aprobación (nivel asignado + límites diario/mensual por FechaAprobacion).
            return _aprobacion.AsegurarAprobadorActualAsync(
                new DocumentoAprobable(
                    AprobacionTipos.OrdenCompra,
                    orden.PlantaCode,
                    orden.Code,
                    orden.UnidadNegocioCode,
                    orden.MonedaCode,
                    orden.TotalCompra),
                cancellationToken);
        }
    }
}
