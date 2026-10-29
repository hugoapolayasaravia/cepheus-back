using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.UpdateTrabajadorFormacion
{
    public record UpdateTrabajadorFormacionCommand(
        long Id,
        string? TrabajadorCode,
        string? NivelEducativoCode,
        string? GradoInstruccionCode,
        string? TituloCode,
        string? EspecialidadCode,
        string? TipoCentroFormacionCode,
        string? ModalidadFormativaCode,
        byte[] RowVersion
    ) : IRequest<TrabajadorFormacionResponse>;
}
