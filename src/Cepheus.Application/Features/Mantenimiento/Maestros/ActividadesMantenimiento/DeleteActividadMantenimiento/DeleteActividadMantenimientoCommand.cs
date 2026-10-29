using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.DeleteActividadMantenimiento
{
    public record DeleteActividadMantenimientoCommand(string ActividadCode, string ActividadCubiertaCode) : IRequest;
}
