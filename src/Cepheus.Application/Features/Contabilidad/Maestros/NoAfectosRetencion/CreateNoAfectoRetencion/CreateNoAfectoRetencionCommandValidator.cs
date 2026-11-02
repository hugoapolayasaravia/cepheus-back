using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.CreateNoAfectoRetencion
{
    public class CreateNoAfectoRetencionCommandValidator : AbstractValidator<CreateNoAfectoRetencionCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateNoAfectoRetencionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Ruc)
                .NotEmpty().WithMessage("El RUC es obligatorio.")
                .Length(11).WithMessage("El RUC debe tener 11 caracteres.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("El tipo es obligatorio.")
                .Length(1).WithMessage("El tipo debe tener 1 carácter.");

            RuleFor(x => x)
                .MustAsync(BeUniqueKey).WithMessage("Ya existe un registro con ese RUC y tipo.")
                .When(x => !string.IsNullOrWhiteSpace(x.Ruc) && !string.IsNullOrWhiteSpace(x.Type));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(70).WithMessage("El nombre no puede exceder los 70 caracteres.");

            RuleFor(x => x.Resolution)
                .NotEmpty().WithMessage("La resolución es obligatoria.")
                .MaximumLength(50).WithMessage("La resolución no puede exceder los 50 caracteres.");
        }

        private async Task<bool> BeUniqueKey(CreateNoAfectoRetencionCommand x, CancellationToken cancellationToken)
            => !await _uow.Contabilidad.Maestros.NoAfectosRetencion.Query()
                .AnyAsync(s => s.Ruc == x.Ruc.Trim() && s.Type == x.Type.Trim().ToUpper(), cancellationToken);
    }
}
