using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.CreateCategoriaTrabajador
{
    public record CreateCategoriaTrabajadorCommand(
        string Name
    ) : IRequest<CategoriaTrabajadorResponse>;
}