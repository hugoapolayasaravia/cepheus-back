using Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.GetModalidadFormativaByCode
{
    public record GetModalidadFormativaByCodeQuery(string Code) : IRequest<ModalidadFormativaResponse>;
}