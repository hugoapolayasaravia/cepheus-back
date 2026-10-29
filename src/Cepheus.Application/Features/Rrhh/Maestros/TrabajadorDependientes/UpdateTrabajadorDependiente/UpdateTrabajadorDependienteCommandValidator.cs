using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.UpdateTrabajadorDependiente
{
    public class UpdateTrabajadorDependienteCommandValidator
        : AbstractValidator<UpdateTrabajadorDependienteCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorDependienteCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El Id del dependiente es obligatorio.");

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El trabajador es obligatorio.")
                .Length(5)
                .WithMessage("El código de trabajador debe tener 5 caracteres.")
                .MustAsync(TrabajadorExists)
                .WithMessage("El trabajador indicado no existe.");

            RuleFor(x => x.Nombre)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El nombre del dependiente es obligatorio.")
                .MaximumLength(150)
                .WithMessage("El nombre del dependiente no puede superar los 150 caracteres.");

            RuleFor(x => x.ParentescoCode)
                .MustAsync(ParentescoExists)
                .WithMessage("El parentesco indicado no existe.");

            RuleFor(x => x.FechaNacimiento)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .When(x => x.FechaNacimiento.HasValue)
                .WithMessage("La fecha de nacimiento no puede ser futura.");

            RuleFor(x => x.Documento)
                .MaximumLength(20)
                .WithMessage("El documento no puede superar los 20 caracteres.");

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> TrabajadorExists(
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return await _uow.Rrhh.Maestros.Trabajadores
                .Query()
                .AnyAsync(
                    x => x.Code == trabajadorCode.Trim(),
                    cancellationToken);
        }

        private async Task<bool> ParentescoExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.Parentescos
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }
    }
}
