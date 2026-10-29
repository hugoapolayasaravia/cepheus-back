using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.UpdateTrabajadorSalud
{
    public class UpdateTrabajadorSaludCommandValidator
        : AbstractValidator<UpdateTrabajadorSaludCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorSaludCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El Id del registro es obligatorio.");

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El trabajador es obligatorio.")
                .Length(5)
                .WithMessage("El código de trabajador debe tener 5 caracteres.")
                .MustAsync(TrabajadorExists)
                .WithMessage("El trabajador indicado no existe.")
                .MustAsync(TrabajadorSaludDoesNotExist)
                .WithMessage("El trabajador ya tiene otro registro de información de salud.");

            RuleFor(x => x.TipoSangreCode)
                .MustAsync(TipoSangreExists)
                .WithMessage("El tipo de sangre indicado no existe.");

            RuleFor(x => x.AlergiaCode)
                .MustAsync(AlergiaExists)
                .WithMessage("La alergia indicada no existe.");

            RuleFor(x => x.Otros)
                .MaximumLength(500)
                .WithMessage("El campo Otros no puede superar los 500 caracteres.");

            RuleFor(x => x.FechaEvaluacionMedica)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .When(x => x.FechaEvaluacionMedica.HasValue)
                .WithMessage("La fecha de evaluación médica no puede ser futura.");

            RuleFor(x => x.Observaciones)
                .MaximumLength(1000)
                .WithMessage("Las observaciones no pueden superar los 1000 caracteres.");

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

        private async Task<bool> TrabajadorSaludDoesNotExist(
            UpdateTrabajadorSaludCommand command,
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return !await _uow.Rrhh.Maestros.TrabajadorSaluds
                .Query()
                .AnyAsync(
                    x => x.TrabajadorCode == trabajadorCode.Trim()
                             && x.Id != command.Id,
                    cancellationToken);
        }

        private async Task<bool> TipoSangreExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.TiposSangre
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> AlergiaExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.Alergias
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }
    }
}
