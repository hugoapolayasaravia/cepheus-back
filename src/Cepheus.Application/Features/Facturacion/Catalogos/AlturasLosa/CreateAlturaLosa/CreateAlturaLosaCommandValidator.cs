using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.CreateAlturaLosa
{
    public class CreateAlturaLosaCommandValidator : AbstractValidator<CreateAlturaLosaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateAlturaLosaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la altura de losa es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una altura de losa con ese nombre.");

            RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Width).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PolystyreneValue).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PolystyreneWidth).GreaterThanOrEqualTo(0);

            RuleFor(x => x.ProductoTipoCode)
                .NotEmpty().WithMessage("El tipo de producto es obligatorio.")
                .MaximumLength(2);

            RuleFor(x => x.ProductoCode)
                .NotEmpty().WithMessage("El producto es obligatorio.")
                .MaximumLength(4);

            RuleFor(x => x)
                .Must(x => (x.PolystyreneProductoTipoCode is null) == (x.PolystyreneProductoCode is null))
                .WithMessage("Debe indicar tipo y código del producto de poliestireno juntos, o ninguno.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.AlturasLosa.Query()
                .AnyAsync(t => t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
