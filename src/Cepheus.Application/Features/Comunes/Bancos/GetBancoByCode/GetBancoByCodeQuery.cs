using Cepheus.Application.Features.Comunes.Bancos.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Bancos.GetBancoByCode
{
    public record GetBancoByCodeQuery(string Code) : IRequest<BancoResponse>;
}
