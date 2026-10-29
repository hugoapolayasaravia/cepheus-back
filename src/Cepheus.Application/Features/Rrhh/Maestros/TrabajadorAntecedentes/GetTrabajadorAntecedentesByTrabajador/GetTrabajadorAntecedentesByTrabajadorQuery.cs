using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.GetTrabajadorAntecedentesByTrabajador
{
    public record GetTrabajadorAntecedentesByTrabajadorQuery(string TrabajadorCode) : IRequest<TrabajadorAntecedenteResponse?>;
}
