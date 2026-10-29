using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.ToggleChoferStatus
{
    public class ToggleChoferStatusCommandHandler : IRequestHandler<ToggleChoferStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleChoferStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleChoferStatusCommand request, CancellationToken cancellationToken)
        {
            var transportistaCode = request.TransportistaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var chofer = await _uow.Facturacion.Maestros.Choferes.Query()
                .FirstOrDefaultAsync(c => c.TransportistaCode == transportistaCode && c.Code == code, cancellationToken);

            if (chofer is null)
            {
                throw new KeyNotFoundException($"Chofer {transportistaCode}/{code} no encontrado.");
            }

            chofer.IsActive = !chofer.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return chofer.IsActive;
        }
    }
}
