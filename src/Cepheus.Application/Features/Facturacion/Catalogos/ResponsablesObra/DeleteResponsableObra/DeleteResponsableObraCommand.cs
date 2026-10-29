using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.DeleteResponsableObra
{
    public record DeleteResponsableObraCommand(
        string ClienteCode,
        string ObraCode,
        int Code
    ) : IRequest;
}
