using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.DeleteUnidadParada
{
    public record DeleteUnidadParadaCommand(string SubCentroCostoCode, DateTime Fecha) : IRequest;
}
