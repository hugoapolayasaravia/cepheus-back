using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.DeleteUnidadHorometro
{
    public record DeleteUnidadHorometroCommand(string SubCentroCostoCode, DateTime Fecha) : IRequest;
}
