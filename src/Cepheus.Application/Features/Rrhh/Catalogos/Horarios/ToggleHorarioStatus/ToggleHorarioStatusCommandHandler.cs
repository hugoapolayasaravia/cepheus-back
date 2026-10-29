using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Horarios.ToggleHorarioStatus
{
    public class ToggleHorarioStatusCommandHandler : IRequestHandler<ToggleHorarioStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleHorarioStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleHorarioStatusCommand request, CancellationToken cancellationToken)
        {
            var horario = await _uow.Rrhh.Catalogos.Horarios.Query()
                .FirstOrDefaultAsync(h => h.Code == request.Code, cancellationToken);

            if (horario is null)
            {
                throw new KeyNotFoundException($"Horario {request.Code} no encontrado.");
            }

            horario.IsActive = !horario.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return horario.IsActive;
        }
    }
}