using Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.GetTecnicoByCode
{
    public record GetTecnicoByCodeQuery(string TrabajadorCode) : IRequest<TecnicoResponse>;
}
