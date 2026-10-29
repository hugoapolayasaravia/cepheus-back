using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.DeleteTrabajadorBeneficio
{
    public record DeleteTrabajadorBeneficioCommand(long Id) : IRequest;
}
