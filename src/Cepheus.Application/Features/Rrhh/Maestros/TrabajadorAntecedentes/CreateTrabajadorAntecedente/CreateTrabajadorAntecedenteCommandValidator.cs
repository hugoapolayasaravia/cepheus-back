using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.CreateTrabajadorAntecedente
{
    public class CreateTrabajadorAntecedenteCommandValidator
        : AbstractValidator<CreateTrabajadorAntecedenteCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorAntecedenteCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El trabajador es obligatorio.")
                .Length(5)
                .WithMessage("El código de trabajador debe tener 5 caracteres.")
                .MustAsync(TrabajadorExists)
                .WithMessage("El trabajador indicado no existe.")
                .MustAsync(TrabajadorAntecedenteDoesNotExist)
                .WithMessage("El trabajador ya tiene registrado un antecedente.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(1000)
                .WithMessage("La descripción no puede superar los 1000 caracteres.");
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

        private async Task<bool> TrabajadorAntecedenteDoesNotExist(
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return !await _uow.Rrhh.Maestros.TrabajadorAntecedentes
                .Query()
                .AnyAsync(
                    x => x.TrabajadorCode == trabajadorCode.Trim(),
                    cancellationToken);
        }
    }
}
