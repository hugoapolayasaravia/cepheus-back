using Cepheus.Application.Features.Comunes.MotivosDevolucion.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.MotivosDevolucion.GetMotivoDevolucionById
{
    public record GetMotivoDevolucionByIdQuery(string Code) : IRequest<MotivoDevolucionResponse>;
}
