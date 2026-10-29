using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Bancos.ToggleBancoStatus
{
    public class ToggleBancoStatusCommandHandler : IRequestHandler<ToggleBancoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleBancoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleBancoStatusCommand request, CancellationToken cancellationToken)
        {
            var banco = await _uow.Comunes.Bancos.Query()
                .FirstOrDefaultAsync(b => b.Code == request.Code, cancellationToken);

            if (banco is null)
            {
                throw new KeyNotFoundException($"Banco {request.Code} no encontrado.");
            }

            banco.IsActive = !banco.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return banco.IsActive;
        }
    }
}
