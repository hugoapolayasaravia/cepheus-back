using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Clientes.ChangeClienteEstado
{
    /// <summary>
    /// El legacy (CK_MClientes) no documenta un flujo de transición
    /// obligatorio entre Activo/Inactivo/Suspendido/PedidoBloqueado, a
    /// diferencia de OrdenTrabajo en Mantenimiento. Por eso este handler no
    /// restringe transiciones: cualquier estado válido del enum es aceptado.
    /// Si el negocio exige un flujo específico (ej. no poder pasar
    /// directamente de PedidoBloqueado a Activo sin aprobación), avisar para
    /// agregar la validación de transiciones.
    /// </summary>
    public class ChangeClienteEstadoCommandHandler : IRequestHandler<ChangeClienteEstadoCommand, ClienteResponse>
    {
        private readonly IUnitOfWork _uow;

        public ChangeClienteEstadoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ClienteResponse> Handle(ChangeClienteEstadoCommand request, CancellationToken cancellationToken)
        {
            if (!System.Enum.TryParse<EstadoCliente>(request.NuevoEstado, true, out var nuevoEstado))
            {
                throw new ArgumentException($"Estado '{request.NuevoEstado}' no es válido.");
            }

            var cliente = await _uow.Facturacion.Maestros.Clientes.Query()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (cliente is null)
            {
                throw new KeyNotFoundException($"Cliente {request.Code} no encontrado.");
            }

            cliente.Estado = nuevoEstado;

            await _uow.SaveChangesAsync(cancellationToken);

            return Cepheus.Application.Features.Facturacion.Maestros.Clientes.CreateCliente.CreateClienteCommandHandler.Map(cliente);
        }
    }
}
