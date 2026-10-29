using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.ToggleModalidadFormativaStatus
{
    public class ToggleModalidadFormativaStatusCommandHandler
        : IRequestHandler<ToggleModalidadFormativaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleModalidadFormativaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleModalidadFormativaStatusCommand request, CancellationToken cancellationToken)
        {
            var modalidadFormativa = await _uow.Rrhh.Catalogos.ModalidadesFormativas.Query()
                .FirstOrDefaultAsync(m => m.Code == request.Code, cancellationToken);

            if (modalidadFormativa is null)
            {
                throw new KeyNotFoundException($"Modalidad formativa {request.Code} no encontrada.");
            }

            modalidadFormativa.IsActive = !modalidadFormativa.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return modalidadFormativa.IsActive;
        }
    }
}