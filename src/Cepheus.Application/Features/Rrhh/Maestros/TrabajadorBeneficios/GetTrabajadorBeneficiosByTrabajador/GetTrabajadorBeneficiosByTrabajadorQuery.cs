using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.GetTrabajadorBeneficiosByTrabajador
{
    public record GetTrabajadorBeneficiosByTrabajadorQuery(string TrabajadorCode) : IRequest<TrabajadorBeneficioResponse?>;
}
