using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.CreateTrabajadorBeneficio
{
    public record CreateTrabajadorBeneficioCommand(
        string TrabajadorCode,
        bool Cts,
        bool Gratificacion,
        bool Vacaciones,
        bool MovilidadAntesEntrada,
        bool MovilidadDespuesSalida,
        bool Refrigerio,
        bool Cena,
        bool Vale
    ) : IRequest<TrabajadorBeneficioResponse>;
}
