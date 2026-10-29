using Cepheus.Application.Features.Logistica.Catalogos.Compradores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Compradores.UpdateComprador
{
    public record UpdateCompradorCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<CompradorResponse>;
}