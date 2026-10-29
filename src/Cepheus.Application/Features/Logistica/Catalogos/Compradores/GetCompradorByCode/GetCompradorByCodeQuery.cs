using Cepheus.Application.Features.Logistica.Catalogos.Compradores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Compradores.GetCompradorByCode
{
    public record GetCompradorByCodeQuery(string Code) : IRequest<CompradorResponse>;
}