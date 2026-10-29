using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.UpdateAlturaLosa
{
    public class UpdateAlturaLosaCommandValidator : AbstractValidator<UpdateAlturaLosaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateAlturaLosaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la altura de losa es obligatorio.")
                .Length(2).WithMessage("El código debe tener 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la altura de losa es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra altura de losa con ese nombre.");

            RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Width).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PolystyreneValue).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PolystyreneWidth).GreaterThanOrEqualTo(0);

            RuleFor(x => x.ProductoTipoCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.ProductoCode).NotEmpty().MaximumLength(4);

            RuleFor(x => x)
                .Must(x => (x.PolystyreneProductoTipoCode is null) == (x.PolystyreneProductoCode is null))
                .WithMessage("Debe indicar tipo y código del producto de poliestireno juntos, o ninguno.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateAlturaLosaCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.AlturasLosa.Query()
                .AnyAsync(t => t.Code != command.Code && t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
