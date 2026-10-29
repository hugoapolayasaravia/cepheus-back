using Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.GetInspeccionByCode
{
    public record GetInspeccionByCodeQuery(string Code) : IRequest<InspeccionResponse>;
}
