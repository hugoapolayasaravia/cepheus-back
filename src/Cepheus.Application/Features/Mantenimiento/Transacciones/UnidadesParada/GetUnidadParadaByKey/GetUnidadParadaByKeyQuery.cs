using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.GetUnidadParadaByKey
{
    public record GetUnidadParadaByKeyQuery(string SubCentroCostoCode, DateTime Fecha) : IRequest<UnidadParadaResponse>;
}
