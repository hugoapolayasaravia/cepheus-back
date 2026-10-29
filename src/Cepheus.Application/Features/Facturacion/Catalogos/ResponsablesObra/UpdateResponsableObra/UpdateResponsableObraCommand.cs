using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.UpdateResponsableObra
{
    /// <summary>
    /// Actualiza los datos de un responsable existente. La clave compuesta
    /// (Cliente/Obra/Responsable) no se puede modificar: para cambiarla hay
    /// que eliminar el registro y crear uno nuevo.
    /// </summary>
    public record UpdateResponsableObraCommand(
        string ClienteCode,
        string ObraCode,
        int Code,
        string Name,
        string Phone,
        string Position,
        byte[] RowVersion
    ) : IRequest<ResponsableObraResponse>;
}
