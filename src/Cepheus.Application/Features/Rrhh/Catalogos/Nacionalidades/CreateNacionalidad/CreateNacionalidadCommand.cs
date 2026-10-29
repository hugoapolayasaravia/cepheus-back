using Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.CreateNacionalidad
{
    public record CreateNacionalidadCommand(
        string Name
    ) : IRequest<NacionalidadResponse>;
}