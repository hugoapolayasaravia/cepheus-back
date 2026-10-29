using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.ToggleMaquinaStatus
{
    public class ToggleMaquinaStatusCommandHandler : IRequestHandler<ToggleMaquinaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleMaquinaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleMaquinaStatusCommand request, CancellationToken cancellationToken)
        {
            var maquina = await _uow.Mantenimiento.Catalogos.Maquinas.Query()
                .FirstOrDefaultAsync(m => m.Code == request.Code, cancellationToken);

            if (maquina is null)
            {
                throw new KeyNotFoundException($"Máquina {request.Code} no encontrada.");
            }

            maquina.IsActive = !maquina.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return maquina.IsActive;
        }
    }
}
