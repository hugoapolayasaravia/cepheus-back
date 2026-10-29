using Cepheus.Application.Features.Comunes.Bancos.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Bancos.UpdateBanco
{
    public record UpdateBancoCommand(
       string Code,
       string Name,
       byte[] RowVersion
   ) : IRequest<BancoResponse>;
}
