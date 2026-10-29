using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.ToggleTipoPensionStatus
{
    public class ToggleTipoPensionStatusCommandHandler : IRequestHandler<ToggleTipoPensionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoPensionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoPensionStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoPension = await _uow.Rrhh.Catalogos.TiposPension.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoPension is null)
            {
                throw new KeyNotFoundException($"Tipo de pensión {request.Code} no encontrado.");
            }

            tipoPension.IsActive = !tipoPension.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoPension.IsActive;
        }
    }
}