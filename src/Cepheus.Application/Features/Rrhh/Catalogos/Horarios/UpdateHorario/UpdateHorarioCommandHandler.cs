using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Horarios.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Horarios.UpdateHorario
{
    public class UpdateHorarioCommandHandler : IRequestHandler<UpdateHorarioCommand, HorarioResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateHorarioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<HorarioResponse> Handle(UpdateHorarioCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.Horarios.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Horario {request.Code} no encontrado.");
            }

            var horario = new Horario
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.Horarios.Update(horario);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El horario fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new HorarioResponse
            {
                Code = horario.Code,
                Name = horario.Name,
                IsActive = horario.IsActive,
                CreatedAt = horario.CreatedAt,
                UpdatedAt = horario.UpdatedAt,
                RowVersion = horario.RowVersion
            };
        }
    }
}