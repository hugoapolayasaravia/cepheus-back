using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.UpdateCategoriaTrabajador
{
    public record UpdateCategoriaTrabajadorCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<CategoriaTrabajadorResponse>;
}