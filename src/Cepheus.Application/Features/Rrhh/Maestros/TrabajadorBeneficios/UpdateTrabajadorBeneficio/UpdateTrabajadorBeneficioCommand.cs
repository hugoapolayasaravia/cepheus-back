using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.UpdateTrabajadorBeneficio
{
    public record UpdateTrabajadorBeneficioCommand(
        long Id,
        string? TrabajadorCode,
        bool Cts,
        bool Gratificacion,
        bool Vacaciones,
        bool MovilidadAntesEntrada,
        bool MovilidadDespuesSalida,
        bool Refrigerio,
        bool Cena,
        bool Vale,
        byte[] RowVersion
    ) : IRequest<TrabajadorBeneficioResponse>;
}
