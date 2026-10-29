using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.GetAnexoByCode
{
    public record GetAnexoByCodeQuery(string TipoAnexoCode, string Code) : IRequest<AnexoResponse>;
}
