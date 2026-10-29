using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Obras.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Obras.ChangeObraEstado
{
    /// <summary>
    /// Igual criterio que ChangeClienteEstadoCommandHandler: el legacy
    /// (CK_MObras) no documenta un flujo de transición obligatorio, así que no
    /// se restringen transiciones. Al pasar a Terminada se completan
    /// CompletionDate/CompletionUser automáticamente si no tenían valor, mismo
    /// criterio que Proveedor.DeactivatedAt/DeactivatedBy.
    /// </summary>
    public class ChangeObraEstadoCommandHandler : IRequestHandler<ChangeObraEstadoCommand, ObraResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public ChangeObraEstadoCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<ObraResponse> Handle(ChangeObraEstadoCommand request, CancellationToken cancellationToken)
        {
            var clienteCode = request.ClienteCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            if (!System.Enum.TryParse<EstadoObra>(request.NuevoEstado, true, out var nuevoEstado))
            {
                throw new ArgumentException($"Estado '{request.NuevoEstado}' no es válido.");
            }

            var obra = await _uow.Facturacion.Maestros.Obras.Query()
                .FirstOrDefaultAsync(o => o.ClienteCode == clienteCode && o.Code == code, cancellationToken);

            if (obra is null)
            {
                throw new KeyNotFoundException($"Obra {clienteCode}/{code} no encontrada.");
            }

            obra.Estado = nuevoEstado;

            if (nuevoEstado == EstadoObra.Terminada)
            {
                obra.CompletionDate ??= DateTime.UtcNow;
                obra.CompletionUser ??= _currentUser.FullName;
            }
            else
            {
                obra.CompletionDate = null;
                obra.CompletionUser = null;
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return Cepheus.Application.Features.Facturacion.Maestros.Obras.CreateObra.CreateObraCommandHandler.Map(obra);
        }
    }
}
