using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.CreateTipoCuentaContable
{
    public class CreateTipoCuentaContableCommandValidator : AbstractValidator<CreateTipoCuentaContableCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoCuentaContableCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un tipo de cuenta contable con esa descripción.");

        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Contabilidad.Catalogos.TiposCuentaContable.Query()
                .AnyAsync(s => s.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
