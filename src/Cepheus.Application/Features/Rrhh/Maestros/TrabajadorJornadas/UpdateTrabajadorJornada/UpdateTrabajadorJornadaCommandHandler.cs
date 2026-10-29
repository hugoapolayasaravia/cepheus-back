using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.UpdateTrabajadorJornada
{
    public class UpdateTrabajadorJornadaCommandHandler : IRequestHandler<UpdateTrabajadorJornadaCommand, TrabajadorJornadaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorJornadaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorJornadaResponse> Handle(UpdateTrabajadorJornadaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorJornadas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorJornada {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorJornada
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                HorarioCode = request.HorarioCode,
                HorasExtras = request.HorasExtras,
                HorasExt40 = request.HorasExt40,
                HorasExtCon = request.HorasExtCon,
                HorasExtCon125 = request.HorasExtCon125,
                HorasExtCon135 = request.HorasExtCon135,
                ControlHorario = request.ControlHorario,
                HorarioOrdinario = request.HorarioOrdinario,
                HorarioNocturno = request.HorarioNocturno,
                JornadaMaxima = request.JornadaMaxima,
                RegimenAlternativo = request.RegimenAlternativo,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorJornadas.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorJornada.CreateTrabajadorJornadaCommandHandler.Map(entidad);
        }
    }
}
