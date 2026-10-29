using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Bancos.CreateBanco
{
    public class CreateBancoCommandValidator : AbstractValidator<CreateBancoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateBancoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del banco es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un banco con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Comunes.Bancos.Query()
                .AnyAsync(b => b.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
