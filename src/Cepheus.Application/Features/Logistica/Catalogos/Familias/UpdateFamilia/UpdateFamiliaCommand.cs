using Cepheus.Application.Features.Logistica.Catalogos.Familias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Familias.UpdateFamilia
{
    // Code viaja como identificador (viene de la ruta), no es editable:
    // es la PK natural y TMSubFamilias depende de él como FK.
    public record UpdateFamiliaCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<FamiliaResponse>;
}