using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposVale.ToggleTipoValeStatus
{
    public class ToggleTipoValeStatusCommandHandler : IRequestHandler<ToggleTipoValeStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoValeStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoValeStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoVale = await _uow.Logistica.Catalogos.TiposVale.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoVale is null)
            {
                throw new KeyNotFoundException($"Tipo de vale {request.Code} no encontrado.");
            }

            tipoVale.IsActive = !tipoVale.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoVale.IsActive;
        }
    }
}