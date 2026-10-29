using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.ToggleTipoHojaCorteStatus
{
    public class ToggleTipoHojaCorteStatusCommandHandler : IRequestHandler<ToggleTipoHojaCorteStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoHojaCorteStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoHojaCorteStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoHojaCorte = await _uow.Facturacion.Catalogos.TiposHojaCorte.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoHojaCorte is null)
            {
                throw new KeyNotFoundException($"Tipo de hoja de corte {request.Code} no encontrado.");
            }

            tipoHojaCorte.IsActive = !tipoHojaCorte.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoHojaCorte.IsActive;
        }
    }
}
