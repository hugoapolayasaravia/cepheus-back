using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.CreateResponsableObra
{
    public class CreateResponsableObraCommandValidator : AbstractValidator<CreateResponsableObraCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateResponsableObraCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.ClienteCode)
                .NotEmpty().WithMessage("El cliente es obligatorio.")
                .MaximumLength(5).WithMessage("El código de cliente no puede exceder los 5 caracteres.");

            RuleFor(x => x.ObraCode)
                .NotEmpty().WithMessage("La obra es obligatoria.")
                .MaximumLength(3).WithMessage("El código de obra no puede exceder los 3 caracteres.");

            RuleFor(x => x)
                .MustAsync(ObraExists)
                .When(x => !string.IsNullOrWhiteSpace(x.ClienteCode) && !string.IsNullOrWhiteSpace(x.ObraCode))
                .WithMessage("La obra indicada no existe para el cliente.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del responsable es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre del responsable no puede exceder los 50 caracteres.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("El teléfono del responsable es obligatorio.")
                .MaximumLength(50).WithMessage("El teléfono del responsable no puede exceder los 50 caracteres.");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("El cargo del responsable es obligatorio.")
                .MaximumLength(50).WithMessage("El cargo del responsable no puede exceder los 50 caracteres.");
        }

        private async Task<bool> ObraExists(CreateResponsableObraCommand c, CancellationToken cancellationToken)
        {
            var clienteCode = c.ClienteCode.Trim().ToUpperInvariant();
            var obraCode = c.ObraCode.Trim().ToUpperInvariant();

            return await _uow.Facturacion.Maestros.Obras.Query()
                .AnyAsync(o => o.ClienteCode == clienteCode && o.Code == obraCode, cancellationToken);
        }
    }
}
