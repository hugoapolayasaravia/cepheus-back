using Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.CreateTipoCentroFormacion
{
    public record CreateTipoCentroFormacionCommand(
        string Name
    ) : IRequest<TipoCentroFormacionResponse>;
}