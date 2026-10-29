using Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.GetSubCentroEjecutorByCode
{
    public record GetSubCentroEjecutorByCodeQuery(string Code) : IRequest<SubCentroEjecutorResponse>;
}
