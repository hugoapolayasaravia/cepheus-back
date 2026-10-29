using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Roles.CreateRole
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateRoleCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
                .MaximumLength(100)
                .MustAsync(BeUniqueName).WithMessage("Ya existe un rol con ese nombre.");

            RuleFor(x => x.Description)
                .MaximumLength(300);
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Administracion.Roles.Query()
                .AnyAsync(r => r.Name == name.Trim(), cancellationToken);
    }

}
