using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.GetTrabajadorJornadasByTrabajador
{
    public class GetTrabajadorJornadasByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorJornadasByTrabajadorQuery, TrabajadorJornadaResponse?>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorJornadasByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorJornadaResponse?> Handle(GetTrabajadorJornadasByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorJornadas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.TrabajadorCode == request.TrabajadorCode, cancellationToken);

            return entidad is null ? null : Map(entidad);
        }

        private static TrabajadorJornadaResponse Map(TrabajadorJornada e) => new()
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
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
