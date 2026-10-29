using Cepheus.Application.Features.Administracion.Programas.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Programas.GetProgramaById
{
    public record GetProgramaByIdQuery(int Id) : IRequest<ProgramaResponse>;
}
