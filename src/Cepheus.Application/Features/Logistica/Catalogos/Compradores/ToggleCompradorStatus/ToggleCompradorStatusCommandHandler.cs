using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Compradores.ToggleCompradorStatus
{
    public class ToggleCompradorStatusCommandHandler : IRequestHandler<ToggleCompradorStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleCompradorStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleCompradorStatusCommand request, CancellationToken cancellationToken)
        {
            var comprador = await _uow.Logistica.Catalogos.Compradores.Query()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (comprador is null)
            {
                throw new KeyNotFoundException($"Comprador {request.Code} no encontrado.");
            }

            comprador.IsActive = !comprador.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return comprador.IsActive;
        }
    }
}