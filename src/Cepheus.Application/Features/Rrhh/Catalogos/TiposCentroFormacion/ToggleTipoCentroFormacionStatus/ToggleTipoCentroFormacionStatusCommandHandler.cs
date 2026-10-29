using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.ToggleTipoCentroFormacionStatus
{
    public class ToggleTipoCentroFormacionStatusCommandHandler
        : IRequestHandler<ToggleTipoCentroFormacionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoCentroFormacionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoCentroFormacionStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoCentroFormacion = await _uow.Rrhh.Catalogos.TiposCentroFormacion.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoCentroFormacion is null)
            {
                throw new KeyNotFoundException($"Tipo de centro de formación {request.Code} no encontrado.");
            }

            tipoCentroFormacion.IsActive = !tipoCentroFormacion.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoCentroFormacion.IsActive;
        }
    }
}