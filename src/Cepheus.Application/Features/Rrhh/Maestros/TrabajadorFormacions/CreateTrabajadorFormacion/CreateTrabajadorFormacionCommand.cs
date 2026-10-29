using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.CreateTrabajadorFormacion
{
    public record CreateTrabajadorFormacionCommand(
        string TrabajadorCode,
        string? NivelEducativoCode,
        string? GradoInstruccionCode,
        string? TituloCode,
        string? EspecialidadCode,
        string? TipoCentroFormacionCode,
        string? ModalidadFormativaCode
    ) : IRequest<TrabajadorFormacionResponse>;
}
