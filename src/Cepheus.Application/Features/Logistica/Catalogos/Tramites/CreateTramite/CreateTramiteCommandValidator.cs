using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Tramites.CreateTramite
{
    public class CreateTramiteCommandValidator : AbstractValidator<CreateTramiteCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTramiteCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del trámite es obligatorio.")
                .MaximumLength(20).WithMessage("El nombre no puede exceder los 20 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una familia con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.Tramites.Query()
                .AnyAsync(f => f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);

    }
}