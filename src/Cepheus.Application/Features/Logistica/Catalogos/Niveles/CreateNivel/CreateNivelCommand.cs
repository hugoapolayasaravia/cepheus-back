using Cepheus.Application.Features.Logistica.Catalogos.Niveles.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Niveles.CreateNivel
{
    /// <summary>Code no se recibe: se genera correlativo (ver SequentialCodeGenerator).</summary>
    public record CreateNivelCommand(
        string Name,
        DateTime? FechaInicio,
        DateTime? FechaFin
    ) : IRequest<NivelResponse>;
}
