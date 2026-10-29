using Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.UpdateNacionalidad
{
    public record UpdateNacionalidadCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<NacionalidadResponse>;
}