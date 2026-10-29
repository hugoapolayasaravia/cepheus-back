using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.GetCategoriaTrabajadorByCode
{
    public record GetCategoriaTrabajadorByCodeQuery(string Code) : IRequest<CategoriaTrabajadorResponse>;
}