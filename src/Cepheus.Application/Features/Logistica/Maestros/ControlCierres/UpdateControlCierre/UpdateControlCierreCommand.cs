using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ControlCierres.UpdateControlCierre
{
    // Solo se puede corregir el monto de diferencia y la fecha; Planta y
    // Período son la identidad del registro y no se editan (para "mover" un
    // cierre a otro período se elimina y se crea de nuevo).
    public record UpdateControlCierreCommand(
        string PlantaCode,
        string PeriodCode,
        DateTime ClosureDate,
        decimal DifferenceAmount
    ) : IRequest<ControlCierreResponse>;
}
