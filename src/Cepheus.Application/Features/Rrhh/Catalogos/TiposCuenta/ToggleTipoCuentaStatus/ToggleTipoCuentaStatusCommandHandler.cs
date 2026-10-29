using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.ToggleTipoCuentaStatus
{
    public class ToggleTipoCuentaStatusCommandHandler : IRequestHandler<ToggleTipoCuentaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoCuentaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoCuentaStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoCuenta = await _uow.Rrhh.Catalogos.TiposCuenta.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoCuenta is null)
            {
                throw new KeyNotFoundException($"Tipo de cuenta {request.Code} no encontrado.");
            }

            tipoCuenta.IsActive = !tipoCuenta.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoCuenta.IsActive;
        }
    }
}