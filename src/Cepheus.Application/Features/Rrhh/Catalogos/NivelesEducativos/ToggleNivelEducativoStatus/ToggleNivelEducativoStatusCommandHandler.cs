using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.ToggleNivelEducativoStatus
{
    public class ToggleNivelEducativoStatusCommandHandler : IRequestHandler<ToggleNivelEducativoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleNivelEducativoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleNivelEducativoStatusCommand request, CancellationToken cancellationToken)
        {
            var nivelEducativo = await _uow.Rrhh.Catalogos.NivelesEducativos.Query()
                .FirstOrDefaultAsync(n => n.Code == request.Code, cancellationToken);

            if (nivelEducativo is null)
            {
                throw new KeyNotFoundException($"Nivel educativo {request.Code} no encontrado.");
            }

            nivelEducativo.IsActive = !nivelEducativo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return nivelEducativo.IsActive;
        }
    }
}