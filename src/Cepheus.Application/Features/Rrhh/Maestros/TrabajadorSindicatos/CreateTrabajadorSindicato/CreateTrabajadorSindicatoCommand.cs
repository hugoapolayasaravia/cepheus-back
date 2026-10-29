using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.CreateTrabajadorSindicato
{
    public record CreateTrabajadorSindicatoCommand(
        string TrabajadorCode,
        bool Afiliado
    ) : IRequest<TrabajadorSindicatoResponse>;
}
