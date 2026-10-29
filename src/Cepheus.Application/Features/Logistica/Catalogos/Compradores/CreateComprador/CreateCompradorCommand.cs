using Cepheus.Application.Features.Logistica.Catalogos.Compradores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Compradores.CreateComprador
{
    public record CreateCompradorCommand(
        string Name
    ) : IRequest<CompradorResponse>;
}