using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.CreateTrabajadorJornada
{
    public record CreateTrabajadorJornadaCommand(
        string TrabajadorCode,
        string? HorarioCode,
        bool HorasExtras,
        bool HorasExt40,
        bool HorasExtCon,
        decimal HorasExtCon125,
        decimal HorasExtCon135,
        bool ControlHorario,
        bool HorarioOrdinario,
        bool HorarioNocturno,
        bool JornadaMaxima,
        bool RegimenAlternativo
    ) : IRequest<TrabajadorJornadaResponse>;
}
