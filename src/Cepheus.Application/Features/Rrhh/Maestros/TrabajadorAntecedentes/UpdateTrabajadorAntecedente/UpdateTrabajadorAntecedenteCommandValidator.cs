using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.UpdateTrabajadorAntecedente
{
    public class UpdateTrabajadorAntecedenteCommandValidator
        : AbstractValidator<UpdateTrabajadorAntecedenteCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorAntecedenteCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El Id del antecedente es obligatorio.");

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

        private async Task<bool> TrabajadorAntecedenteDoesNotExist(
            UpdateTrabajadorAntecedenteCommand command,
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return !await _uow.Rrhh.Maestros.TrabajadorAntecedentes
                .Query()
                .AnyAsync(
                    x => x.TrabajadorCode == trabajadorCode.Trim()
                         && x.Id != command.Id,
                    cancellationToken);
        }
    }
}
