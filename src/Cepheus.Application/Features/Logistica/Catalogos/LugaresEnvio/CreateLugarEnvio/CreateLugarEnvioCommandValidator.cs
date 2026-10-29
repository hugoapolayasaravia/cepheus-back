using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.CreateLugarEnvio
{
    public class CreateLugarEnvioCommandValidator : AbstractValidator<CreateLugarEnvioCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateLugarEnvioCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del lugar de envío es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una familia con ese nombre.");

            RuleFor(x => x.Address)
                .MaximumLength(150).WithMessage("La dirección no puede exceder los 150 caracteres.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.LugaresEnvio.Query()
                .AnyAsync(f => f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}