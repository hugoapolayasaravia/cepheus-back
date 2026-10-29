using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.UpdateAjusteInventario;

public sealed class UpdateAjusteInventarioCommandValidator : AbstractValidator<UpdateAjusteInventarioCommand>
{
    public UpdateAjusteInventarioCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(6);
        RuleFor(x => x.FechaEntrega).NotEmpty()
            .WithMessage("La fecha de entrega es obligatoria.");
        RuleFor(x => x.Observacion).MaximumLength(1000);
    }
}
