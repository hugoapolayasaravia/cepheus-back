using Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.UpdateTipoCentroFormacion
{
    public record UpdateTipoCentroFormacionCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoCentroFormacionResponse>;
}