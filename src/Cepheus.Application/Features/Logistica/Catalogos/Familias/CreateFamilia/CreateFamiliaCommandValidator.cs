using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Familias.CreateFamilia
{
    public class CreateFamiliaCommandValidator : AbstractValidator<CreateFamiliaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateFamiliaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("El código de la familia es obligatorio.")
               .MaximumLength(10).WithMessage("El código no puede exceder los 10 caracteres.")
               .MustAsync(BeUniqueCode).WithMessage("Ya existe una familia con ese código.");


            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la familia es obligatorio.")
                .MaximumLength(60).WithMessage("El nombre no puede exceder los 60 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una familia con ese nombre.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
             => !await _uow.Logistica.Catalogos.Familias.Query()
                 .AnyAsync(p => p.Code == code.Trim().ToUpper(), cancellationToken);


        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.Familias.Query()
                .AnyAsync(f => f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}