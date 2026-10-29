using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.ChangeEstadoOrdenServicio;

public sealed class ChangeEstadoOrdenServicioCommandValidator : AbstractValidator<ChangeEstadoOrdenServicioCommand>
{
    public ChangeEstadoOrdenServicioCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(6);
        RuleFor(x => x.NuevoEstado)
            .NotEmpty().WithMessage("El nuevo estado es obligatorio.")
            .Must(e => OrdenServicioRules.TryParseEstado(e, out _))
            .WithMessage("El estado indicado no es válido. Valores: Pendiente, Aprobado, Procesado, Cerrado, Anulado.");
    }
}
