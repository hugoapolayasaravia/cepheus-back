using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.UpdateUnidadNegocio
{
    public class UpdateUnidadNegocioCommandValidator : AbstractValidator<UpdateUnidadNegocioCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateUnidadNegocioCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la unidad de negocio es obligatorio.")
                .Length(6).WithMessage("El código debe tener 6 caracteres.");

            RuleFor(x => x.Name)
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("Ya existe otra unidad de negocio con ese nombre.");

            RuleFor(x => x.ParentCode)
                .MustAsync(ParentExists).WithMessage("La unidad de negocio padre indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ParentCode));

            RuleFor(x => x)
                .Must(x => !string.Equals(x.Code?.Trim(), x.ParentCode?.Trim(), StringComparison.OrdinalIgnoreCase))
                .WithMessage("Una unidad de negocio no puede ser padre de sí misma.")
                .When(x => !string.IsNullOrWhiteSpace(x.ParentCode));

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateUnidadNegocioCommand command, string? name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.UnidadesNegocio.Query()
                .AnyAsync(u => u.Code != command.Code && u.Name != null && u.Name.ToLower() == name!.Trim().ToLower(), cancellationToken);

        private async Task<bool> ParentExists(string? parentCode, CancellationToken cancellationToken)
            => await _uow.Logistica.Catalogos.UnidadesNegocio.Query()
                .AnyAsync(u => u.Code == parentCode!.Trim().ToUpper(), cancellationToken);
    }
}