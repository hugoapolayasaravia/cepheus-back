// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/ChangeEstadoOrdenCompraAdministracion/ChangeEstadoOrdenCompraAdministracionCommandHandler.cs
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.GetAprobadoresPorCombinacion;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.ResolverRangoAprobacion;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.ChangeEstadoOrdenCompraAdm
{
    public class ChangeEstadoOrdenCompraAdmCommandHandler
        : IRequestHandler<ChangeEstadoOrdenCompraAdmCommand, OrdenCompraAdmResponse>
    {
        private static readonly Dictionary<EstadoOrdenCompraAdministracion, EstadoOrdenCompraAdministracion[]> ValidTransitions = new()
        {
            [EstadoOrdenCompraAdministracion.Pendiente] = new[] { EstadoOrdenCompraAdministracion.Aprobado, EstadoOrdenCompraAdministracion.Anulado },
            [EstadoOrdenCompraAdministracion.Aprobado] = new[] { EstadoOrdenCompraAdministracion.Cerrado, EstadoOrdenCompraAdministracion.Anulado },
            [EstadoOrdenCompraAdministracion.Cerrado] = Array.Empty<EstadoOrdenCompraAdministracion>(),
            [EstadoOrdenCompraAdministracion.Anulado] = Array.Empty<EstadoOrdenCompraAdministracion>()
        };

        private readonly IUnitOfWork _uow;
        private readonly ISender _sender;
        private readonly ICurrentUserService _currentUser;

        public ChangeEstadoOrdenCompraAdmCommandHandler(
            IUnitOfWork uow, ISender sender, ICurrentUserService currentUser)
        {
            _uow = uow;
            _sender = sender;
            _currentUser = currentUser;
        }

        public async Task<OrdenCompraAdmResponse> Handle(
            ChangeEstadoOrdenCompraAdmCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim().ToUpperInvariant();

            if (!System.Enum.TryParse<EstadoOrdenCompraAdministracion>(request.NuevoEstado, true, out var nuevoEstado)
                || !System.Enum.IsDefined(nuevoEstado))
            {
                throw new ArgumentException($"Estado '{request.NuevoEstado}' no es válido.");
            }

            var orden = await _uow.Logistica.Transacciones.OrdenesCompraAdministracion.Query()
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Code == code, cancellationToken);

            if (orden is null)
            {
                throw new KeyNotFoundException($"Orden de Compra de Administración {code} no encontrada.");
            }

            if (!ValidTransitions[orden.Estado].Contains(nuevoEstado))
            {
                throw new InvalidOperationException(
                    $"No se puede pasar de '{orden.Estado}' a '{nuevoEstado}'. " +
                    $"Transiciones válidas: {string.Join(", ", ValidTransitions[orden.Estado])}.");
            }

            if (nuevoEstado == EstadoOrdenCompraAdministracion.Aprobado)
            {
                await ValidarAprobadorAsync(orden, cancellationToken);
                orden.AprobadoPor = _currentUser.FullName;
                orden.FechaAprobacion = DateTime.UtcNow;
            }

            orden.Estado = nuevoEstado;

            await _uow.SaveChangesAsync(cancellationToken);

            return OrdenCompraAdmMapper.Map(orden);
        }

        private async Task ValidarAprobadorAsync(OrdenCompraAdministracion orden, CancellationToken cancellationToken)
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
                    "Tu usuario no tiene un Trabajador de RRHH vinculado — no puedes aprobar Órdenes de Compra de Administración.");
            }

            var rango = await _sender.Send(
                new ResolverRangoAprobacionQuery(
                    OrdenCompraAdmRules.TipoTransaccionOCA,
                    orden.UnidadNegocioCode,
                    orden.MonedaCode,
                    orden.TotalCompra),
                cancellationToken);

            if (rango.RangoAplicable is null)
            {
                throw new InvalidOperationException(
                    $"No existe un rango de aprobación configurado que cubra {orden.TotalCompra:N2} " +
                    $"para la unidad de negocio {orden.UnidadNegocioCode} en {orden.MonedaCode} " +
                    $"(tipo de transacción {OrdenCompraAdmRules.TipoTransaccionOCA}).");
            }

            var aprobadores = await _sender.Send(
                new GetAprobadoresPorCombinacionQuery(
                    rango.RangoAplicable.NivelCode,
                    OrdenCompraAdmRules.TipoTransaccionOCA,
                    rango.UnidadNegocioEfectivaCode,
                    orden.MonedaCode),
                cancellationToken);

            var autorizado = aprobadores.Any(a =>
                a.TrabajadorCode == trabajadorCode ||
                a.SuplenteTrabajadorCode == trabajadorCode ||
                a.SuperiorTrabajadorCode == trabajadorCode);

            if (!autorizado)
            {
                throw new InvalidOperationException(
                    $"No estás autorizado para aprobar esta Orden de Compra de Administración (nivel {rango.RangoAplicable.NivelCode}, " +
                    $"monto {orden.TotalCompra:N2}).");
            }
        }
    }
}
