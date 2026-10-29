using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.ToggleNotaCotizacionPlantillaStatus
{
    public class ToggleNotaCotizacionPlantillaStatusCommandHandler
        : IRequestHandler<ToggleNotaCotizacionPlantillaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleNotaCotizacionPlantillaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleNotaCotizacionPlantillaStatusCommand request, CancellationToken cancellationToken)
        {
            var nota = await _uow.Facturacion.Catalogos.NotasCotizacionPlantilla.Query()
                .FirstOrDefaultAsync(t => t.NegocioCode == request.NegocioCode && t.Code == request.Code, cancellationToken);

            if (nota is null)
            {
                throw new KeyNotFoundException($"Nota de cotización {request.NegocioCode}-{request.Code} no encontrada.");
            }

            nota.IsActive = !nota.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return nota.IsActive;
        }
    }
}
