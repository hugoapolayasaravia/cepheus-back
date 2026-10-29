using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.UpdateTrabajadorSindicato
{
    public record UpdateTrabajadorSindicatoCommand(
        long Id,
        string? TrabajadorCode,
        bool Afiliado,
        byte[] RowVersion
    ) : IRequest<TrabajadorSindicatoResponse>;
}
