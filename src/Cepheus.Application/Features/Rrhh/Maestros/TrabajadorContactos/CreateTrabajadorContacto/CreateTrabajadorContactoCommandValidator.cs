using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.CreateTrabajadorContacto
{
    public class CreateTrabajadorContactoCommandValidator : AbstractValidator<CreateTrabajadorContactoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorContactoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El trabajador es obligatorio.")
                .Length(5).WithMessage("El código de trabajador debe tener 5 caracteres.")
                .MustAsync(TrabajadorExists).WithMessage("El trabajador indicado no existe.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del contacto es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");

            RuleFor(x => x.Phone)
                .MaximumLength(30).WithMessage("El teléfono no puede exceder los 30 caracteres.");

            RuleFor(x => x.ParentescoCode)
                .MustAsync(ParentescoExists).WithMessage("El parentesco indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ParentescoCode));
        }

        private async Task<bool> TrabajadorExists(string trabajadorCode, CancellationToken cancellationToken)
            => await _uow.Rrhh.Maestros.Trabajadores.Query()
                .AnyAsync(t => t.Code == trabajadorCode.Trim().ToUpper(), cancellationToken);

        private async Task<bool> ParentescoExists(string? code, CancellationToken cancellationToken)
            => await _uow.Rrhh.Catalogos.Parentescos.Query().AnyAsync(p => p.Code == code!.Trim().ToUpper(), cancellationToken);
    }
}