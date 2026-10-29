using Cepheus.Application.Features.Comunes.ControlesVentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.ControlesVentas.GetControlVentas
{
    /// <summary>
    /// Sin parámetros: siempre retorna la única fila de configuración vigente.
    /// </summary>
    public record GetControlVentasQuery : IRequest<ControlVentasResponse>;
}
