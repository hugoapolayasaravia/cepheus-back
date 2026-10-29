using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.ToggleGradoInstruccionStatus
{
    public class ToggleGradoInstruccionStatusCommandHandler : IRequestHandler<ToggleGradoInstruccionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleGradoInstruccionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleGradoInstruccionStatusCommand request, CancellationToken cancellationToken)
        {
            var gradoInstruccion = await _uow.Rrhh.Catalogos.GradosInstruccion.Query()
                .FirstOrDefaultAsync(g => g.Code == request.Code, cancellationToken);

            if (gradoInstruccion is null)
            {
                throw new KeyNotFoundException($"Grado de instrucción {request.Code} no encontrado.");
            }

            gradoInstruccion.IsActive = !gradoInstruccion.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return gradoInstruccion.IsActive;
        }
    }
}