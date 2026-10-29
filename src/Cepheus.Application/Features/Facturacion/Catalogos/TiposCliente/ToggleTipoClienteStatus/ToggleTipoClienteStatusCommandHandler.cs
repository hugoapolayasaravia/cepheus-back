using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.ToggleTipoClienteStatus
{
    public class ToggleTipoClienteStatusCommandHandler : IRequestHandler<ToggleTipoClienteStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoClienteStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoClienteStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoCliente = await _uow.Facturacion.Catalogos.TiposCliente.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoCliente is null)
            {
                throw new KeyNotFoundException($"Tipo de cliente {request.Code} no encontrado.");
            }

            tipoCliente.IsActive = !tipoCliente.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoCliente.IsActive;
        }
    }
}
