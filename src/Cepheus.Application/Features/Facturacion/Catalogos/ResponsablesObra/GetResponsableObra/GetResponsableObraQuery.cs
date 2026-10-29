using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.GetResponsableObra
{
    public record GetResponsableObraQuery(
        string ClienteCode,
        string ObraCode,
        int Code
    ) : IRequest<ResponsableObraResponse>;
}
