using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Roles.UpdateRole
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateRoleCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
                .MaximumLength(100)
                .MustAsync(BeUniqueName).WithMessage("Ya existe un rol con ese nombre.");

            RuleFor(x => x.Description)
                .MaximumLength(300);

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para validar concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateRoleCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Administracion.Roles.Query()
                .AnyAsync(r => r.Name == name.Trim() && r.Id != command.Id, cancellationToken);
    }

}
