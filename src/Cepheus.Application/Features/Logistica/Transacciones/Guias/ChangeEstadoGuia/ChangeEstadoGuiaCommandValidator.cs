// Cepheus.Application/Features/Logistica/Transacciones/Guias/ChangeEstadoGuia/ChangeEstadoGuiaCommandValidator.cs
using Cepheus.Domain.Logistica.Enum;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.ChangeEstadoGuia
{
    public class ChangeEstadoGuiaCommandValidator : AbstractValidator<ChangeEstadoGuiaCommand>
    {
        public ChangeEstadoGuiaCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().WithMessage("La planta es obligatoria.");
            RuleFor(x => x.Code).NotEmpty().WithMessage("El número de guía es obligatorio.");

            RuleFor(x => x.NuevoEstado)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nuevo estado es obligatorio.")
                .Must(value => System.Enum.TryParse<EstadoGuia>(value, true, out var estado)
                               && System.Enum.IsDefined(typeof(EstadoGuia), estado))
                .WithMessage("El estado indicado no es válido. Valores permitidos: Pendiente, Anulado.");
        }
    }
}
