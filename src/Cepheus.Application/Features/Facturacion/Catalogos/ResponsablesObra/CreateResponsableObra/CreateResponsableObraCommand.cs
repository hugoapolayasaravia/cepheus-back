using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.CreateResponsableObra
{
    /// <summary>
    /// Crea un responsable para una obra. Code (Codigo_Res) es un correlativo
    /// por obra y lo genera el servidor.
    /// </summary>
    public record CreateResponsableObraCommand(
        string ClienteCode,
        string ObraCode,
        string Name,
        string Phone,
        string Position
    ) : IRequest<ResponsableObraResponse>;
}
