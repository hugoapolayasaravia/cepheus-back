using FluentValidation;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.UpdateOTRMaterial
{
    public class UpdateOTRMaterialCommandValidator : AbstractValidator<UpdateOTRMaterialCommand>
    {
        public UpdateOTRMaterialCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.OrdenTrabajoCode).NotEmpty();
            RuleFor(x => x.ArticuloCode).NotEmpty();
            RuleFor(x => x.FechaProceso).NotEmpty();

            RuleFor(x => x.Cantidad).GreaterThanOrEqualTo(0).WithMessage("La cantidad no puede ser negativa.");
            RuleFor(x => x.CostoUnitario).GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.");
            RuleFor(x => x.CostoTotal).GreaterThanOrEqualTo(0).WithMessage("El costo total no puede ser negativo.");

            RuleFor(x => x.EstadoCode)
                .MaximumLength(2).WithMessage("El código de estado no puede exceder los 2 caracteres.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
