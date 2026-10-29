using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.ToggleClasificacionClienteStatus
{
    public class ToggleClasificacionClienteStatusCommandHandler : IRequestHandler<ToggleClasificacionClienteStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleClasificacionClienteStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleClasificacionClienteStatusCommand request, CancellationToken cancellationToken)
        {
            var clasificacionCliente = await _uow.Facturacion.Catalogos.ClasificacionesCliente.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (clasificacionCliente is null)
            {
                throw new KeyNotFoundException($"Clasificación de cliente {request.Code} no encontrada.");
            }

            clasificacionCliente.IsActive = !clasificacionCliente.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return clasificacionCliente.IsActive;
        }
    }
}
