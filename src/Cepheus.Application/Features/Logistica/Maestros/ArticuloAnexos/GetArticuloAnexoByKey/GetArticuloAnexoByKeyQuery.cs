using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.GetArticuloAnexoByKey
{
    public record GetArticuloAnexoByKeyQuery(string SubCentroCostoCode, string ArticuloCode) : IRequest<ArticuloAnexoResponse>;
}
