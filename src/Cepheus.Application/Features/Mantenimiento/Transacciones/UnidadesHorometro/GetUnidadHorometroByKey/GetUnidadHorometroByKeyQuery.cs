using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.GetUnidadHorometroByKey
{
    public record GetUnidadHorometroByKeyQuery(string SubCentroCostoCode, DateTime Fecha) : IRequest<UnidadHorometroResponse>;
}
