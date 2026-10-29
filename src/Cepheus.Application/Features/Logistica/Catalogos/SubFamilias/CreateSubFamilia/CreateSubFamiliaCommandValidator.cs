using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.CreateSubFamilia
{
    public class CreateSubFamiliaCommandValidator : AbstractValidator<CreateSubFamiliaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateSubFamiliaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.FamiliaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La familia es obligatoria.")
                .Length(2).WithMessage("El código de familia debe tener 2 caracteres.")
                .MustAsync(FamiliaExists).WithMessage("La familia indicada no existe.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la subfamilia es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una subfamilia con ese nombre.");
        }

        private async Task<bool> FamiliaExists(string familiaCode, CancellationToken cancellationToken)
            => await _uow.Logistica.Catalogos.Familias.Query()
                .AnyAsync(f => f.Code == familiaCode.Trim().ToUpper(), cancellationToken);

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.SubFamilias.Query()
                .AnyAsync(s => s.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}