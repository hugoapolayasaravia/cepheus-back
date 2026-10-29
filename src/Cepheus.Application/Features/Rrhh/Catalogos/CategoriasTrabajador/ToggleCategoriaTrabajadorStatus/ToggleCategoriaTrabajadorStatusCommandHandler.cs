using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.ToggleCategoriaTrabajadorStatus
{
    public class ToggleCategoriaTrabajadorStatusCommandHandler
        : IRequestHandler<ToggleCategoriaTrabajadorStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleCategoriaTrabajadorStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleCategoriaTrabajadorStatusCommand request, CancellationToken cancellationToken)
        {
            var categoriaTrabajador = await _uow.Rrhh.Catalogos.CategoriasTrabajador.Query()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (categoriaTrabajador is null)
            {
                throw new KeyNotFoundException($"Categoría de trabajador {request.Code} no encontrada.");
            }

            categoriaTrabajador.IsActive = !categoriaTrabajador.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return categoriaTrabajador.IsActive;
        }
    }
}