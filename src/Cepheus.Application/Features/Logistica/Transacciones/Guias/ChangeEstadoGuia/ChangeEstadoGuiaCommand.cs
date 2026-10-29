// Cepheus.Application/Features/Logistica/Transacciones/Guias/ChangeEstadoGuia/ChangeEstadoGuiaCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.ChangeEstadoGuia
{
    public record ChangeEstadoGuiaCommand(
        string PlantaCode,
        string Code,
        string NuevoEstado
    ) : IRequest<GuiaResponse>;
}
