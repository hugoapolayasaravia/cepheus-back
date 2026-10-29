using Cepheus.Application.Features.Comunes.Bancos.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Bancos.CreateBanco
{
    public record CreateBancoCommand(
       string Name
   ) : IRequest<BancoResponse>;
}
