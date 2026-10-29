using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.CreateVale;

public sealed class CreateValeCommandValidator : AbstractValidator<CreateValeCommand>
{
    public CreateValeCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2)
            .WithMessage("La planta es obligatoria.");

        RuleFor(x => x.TipoValeCode).NotEmpty().MaximumLength(3)
            .WithMessage("El tipo de vale es obligatorio.");

        RuleFor(x => x.FechaEntrega).NotEmpty()
            .WithMessage("La fecha de entrega es obligatoria.");

        RuleFor(x => x.UnidadNegocioCode).NotEmpty().MaximumLength(6)
            .WithMessage("La unidad de negocio es obligatoria.");

        RuleFor(x => x.SubCentroCostoCode).MaximumLength(6);
        RuleFor(x => x.SubCentroEjecutorCode).MaximumLength(4);
        RuleFor(x => x.TrabajadorCode).MaximumLength(5);
        RuleFor(x => x.OrdenTrabajoCode).MaximumLength(6);
        RuleFor(x => x.PlantaAfectadaCode).MaximumLength(2);

        RuleForEach(x => x.Detalles).ChildRules(d =>
        {
            d.RuleFor(l => l.ArticuloCode).NotEmpty().MaximumLength(7)
                .WithMessage("El código de artículo es obligatorio.");
            d.RuleFor(l => l.Cantidad).GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a cero.");
            d.RuleFor(l => l.Propiedad01).MaximumLength(7);
        });

        RuleForEach(x => x.MaterialesOrdenTrabajo).ChildRules(m =>
        {
            m.RuleFor(l => l.ArticuloCode).NotEmpty().MaximumLength(7);
            m.RuleFor(l => l.FechaProceso).NotEmpty();
        });
    }
}
