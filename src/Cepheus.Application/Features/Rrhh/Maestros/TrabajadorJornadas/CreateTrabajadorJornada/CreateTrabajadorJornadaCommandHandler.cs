using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.CreateTrabajadorJornada
{
    public class CreateTrabajadorJornadaCommandHandler : IRequestHandler<CreateTrabajadorJornadaCommand, TrabajadorJornadaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorJornadaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorJornadaResponse> Handle(CreateTrabajadorJornadaCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorJornada
            {
                TrabajadorCode = request.TrabajadorCode,
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
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorJornadas.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorJornadaResponse Map(TrabajadorJornada e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            HorarioCode = e.HorarioCode,
            HorasExtras = e.HorasExtras,
            HorasExt40 = e.HorasExt40,
            HorasExtCon = e.HorasExtCon,
            HorasExtCon125 = e.HorasExtCon125,
            HorasExtCon135 = e.HorasExtCon135,
            ControlHorario = e.ControlHorario,
            HorarioOrdinario = e.HorarioOrdinario,
            HorarioNocturno = e.HorarioNocturno,
            JornadaMaxima = e.JornadaMaxima,
            RegimenAlternativo = e.RegimenAlternativo,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
