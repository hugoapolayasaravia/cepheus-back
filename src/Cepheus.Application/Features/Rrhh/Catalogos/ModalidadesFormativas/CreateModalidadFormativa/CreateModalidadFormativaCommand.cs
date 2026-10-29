using Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.CreateModalidadFormativa
{
    public record CreateModalidadFormativaCommand(
        string Name
    ) : IRequest<ModalidadFormativaResponse>;
}