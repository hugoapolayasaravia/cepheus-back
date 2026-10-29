using Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.GetCentroCostoByCode
{
    public record GetCentroCostoByCodeQuery(string Code) : IRequest<CentroCostoResponse>;
}
