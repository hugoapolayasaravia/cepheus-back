using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.UpdateTrabajadorContacto
{
    public class UpdateTrabajadorContactoCommandValidator : AbstractValidator<UpdateTrabajadorContactoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorContactoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del contacto es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");

            RuleFor(x => x.Phone)
                .MaximumLength(30).WithMessage("El teléfono no puede exceder los 30 caracteres.");

            RuleFor(x => x.ParentescoCode)
                .MustAsync(ParentescoExists).WithMessage("El parentesco indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ParentescoCode));
        }

        private async Task<bool> ParentescoExists(string? code, CancellationToken cancellationToken)
            => await _uow.Rrhh.Catalogos.Parentescos.Query().AnyAsync(p => p.Code == code!.Trim().ToUpper(), cancellationToken);
    }
}