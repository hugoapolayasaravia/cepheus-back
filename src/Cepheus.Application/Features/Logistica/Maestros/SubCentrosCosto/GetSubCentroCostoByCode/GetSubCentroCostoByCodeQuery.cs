using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.GetSubCentroCostoByCode
{
    public record GetSubCentroCostoByCodeQuery(string Code) : IRequest<SubCentroCostoResponse>;
}
