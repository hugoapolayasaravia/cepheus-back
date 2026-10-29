using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.UpdateTrabajadorPension
{
    public class UpdateTrabajadorPensionCommandValidator
        : AbstractValidator<UpdateTrabajadorPensionCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorPensionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El id de la información pensionaria es obligatorio.");

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion es obligatorio para control de concurrencia.");

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El trabajador es obligatorio.")
                .Length(5)
                .WithMessage("El código de trabajador debe tener 5 caracteres.")
                .MustAsync(TrabajadorExists)
                .WithMessage("El trabajador indicado no existe.")
                .MustAsync(BeUniqueTrabajador)
                .WithMessage("El trabajador ya tiene otra información pensionaria registrada.");

            RuleFor(x => x.TipoAfiliacionCode)
                .MustAsync(TipoAfiliacionExists)
                .WithMessage("El tipo de afiliación indicado no existe.");

            RuleFor(x => x.AfpCode)
                .MustAsync(AfpExists)
                .WithMessage("La AFP indicada no existe.");

            RuleFor(x => x.RegimenPensionarioCode)
                .MustAsync(RegimenPensionarioExists)
                .WithMessage("El régimen pensionario indicado no existe.");

            RuleFor(x => x.TipoPensionCode)
                .MustAsync(TipoPensionExists)
                .WithMessage("El tipo de pensión indicado no existe.");

            RuleFor(x => x.FechaAfiliacion)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .When(x => x.FechaAfiliacion.HasValue)
                .WithMessage("La fecha de afiliación no puede ser futura.");

            RuleFor(x => x.NumeroAfp)
                .MaximumLength(50)
                .WithMessage("El número de AFP no puede superar los 50 caracteres.");

            RuleFor(x => x.NumeroCarnetSsp)
                .MaximumLength(50)
                .WithMessage("El número de carnet SSP no puede superar los 50 caracteres.");
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

        private async Task<bool> BeUniqueTrabajador(
            UpdateTrabajadorPensionCommand command,
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return !await _uow.Rrhh.Maestros.TrabajadorPensions
                .Query()
                .AnyAsync(
                    x => x.TrabajadorCode == trabajadorCode.Trim()
                         && x.Id != command.Id,
                    cancellationToken);
        }

        private async Task<bool> TipoAfiliacionExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.TiposAfiliacion
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> AfpExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.Afps
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> RegimenPensionarioExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.RegimenesPensionarios
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> TipoPensionExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.TiposPension
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }
    }
}
