using Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.UpdateModalidadFormativa
{
    public record UpdateModalidadFormativaCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<ModalidadFormativaResponse>;
}