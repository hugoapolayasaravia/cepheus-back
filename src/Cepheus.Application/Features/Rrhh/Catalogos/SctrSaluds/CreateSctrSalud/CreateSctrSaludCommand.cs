using Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.CreateSctrSalud
{
    public record CreateSctrSaludCommand(
        string Name
    ) : IRequest<SctrSaludResponse>;
}