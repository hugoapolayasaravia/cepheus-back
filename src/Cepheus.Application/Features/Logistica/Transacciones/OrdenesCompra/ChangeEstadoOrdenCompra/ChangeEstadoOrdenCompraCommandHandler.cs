// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/ChangeEstadoOrdenCompra/ChangeEstadoOrdenCompraCommandHandler.cs
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.GetAprobadoresPorCombinacion;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.ResolverRangoAprobacion;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.ChangeEstadoOrdenCompra
{
    public class ChangeEstadoOrdenCompraCommandHandler : IRequestHandler<ChangeEstadoOrdenCompraCommand, OrdenCompraResponse>
    {
        /// <summary>Código en TipoTransaccion para Orden de Compra (confirmado).</summary>
        private const string TipoTransaccionOC = "OC";

        private static readonly Dictionary<EstadoOrdenCompra, EstadoOrdenCompra[]> ValidTransitions = new()
        {
            [EstadoOrdenCompra.Pendiente] = new[] { EstadoOrdenCompra.Aprobado, EstadoOrdenCompra.Anulado },
            [EstadoOrdenCompra.Aprobado] = new[] { EstadoOrdenCompra.EntregaParcial, EstadoOrdenCompra.Cerrado, EstadoOrdenCompra.Anulado },
            [EstadoOrdenCompra.EntregaParcial] = new[] { EstadoOrdenCompra.Cerrado },
            [EstadoOrdenCompra.Cerrado] = Array.Empty<EstadoOrdenCompra>(),
            [EstadoOrdenCompra.Anulado] = Array.Empty<EstadoOrdenCompra>()
        };

        private readonly IUnitOfWork _uow;
        private readonly ISender _sender;
        private readonly ICurrentUserService _currentUser;

        public ChangeEstadoOrdenCompraCommandHandler(IUnitOfWork uow, ISender sender, ICurrentUserService currentUser)
        {
            _uow = uow;
            _sender = sender;
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
                orden.AprobadoPor = _currentUser.FullName;
                orden.FechaAprobacion = DateTime.UtcNow;
            }

            orden.Estado = nuevoEstado;

            await _uow.SaveChangesAsync(cancellationToken);

            return OrdenCompraMapper.Map(orden);
        }

        private async Task ValidarAprobadorAsync(Domain.Logistica.Transacciones.OrdenCompra orden, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is null)
            {
                throw new InvalidOperationException("No se pudo identificar al usuario autenticado.");
            }

            var trabajadorCode = await _uow.Administracion.Users.Query()
                .AsNoTracking()
                .Where(u => u.Id == _currentUser.UserId.Value)
                .Select(u => u.TrabajadorCode)
                .FirstOrDefaultAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(trabajadorCode))
            {
                throw new InvalidOperationException(
                    "Tu usuario no tiene un Trabajador de RRHH vinculado — no puedes aprobar Órdenes de Compra.");
            }

            var rango = await _sender.Send(
                new ResolverRangoAprobacionQuery(TipoTransaccionOC, orden.UnidadNegocioCode, orden.MonedaCode, orden.TotalCompra),
                cancellationToken);

            if (rango.RangoAplicable is null)
            {
                throw new InvalidOperationException(
                    $"No existe un rango de aprobación configurado que cubra S/ {orden.TotalCompra:N2} " +
                    $"para la unidad de negocio {orden.UnidadNegocioCode} en {orden.MonedaCode}.");
            }

            var aprobadores = await _sender.Send(
                new GetAprobadoresPorCombinacionQuery(
                    rango.RangoAplicable.NivelCode, TipoTransaccionOC, rango.UnidadNegocioEfectivaCode, orden.MonedaCode),
                cancellationToken);

            var autorizado = aprobadores.Any(a =>
                a.TrabajadorCode == trabajadorCode ||
                a.SuplenteTrabajadorCode == trabajadorCode ||
                a.SuperiorTrabajadorCode == trabajadorCode);

            if (!autorizado)
            {
                throw new InvalidOperationException(
                    $"No estás autorizado para aprobar esta Orden de Compra (nivel {rango.RangoAplicable.NivelCode}, " +
                    $"monto S/ {orden.TotalCompra:N2}).");
            }
        }
    }
}