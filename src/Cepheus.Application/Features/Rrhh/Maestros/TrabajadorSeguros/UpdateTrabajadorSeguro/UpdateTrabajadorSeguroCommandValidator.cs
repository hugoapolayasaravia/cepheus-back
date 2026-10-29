using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.UpdateTrabajadorSeguro
{
    public class UpdateTrabajadorSeguroCommandValidator
        : AbstractValidator<UpdateTrabajadorSeguroCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorSeguroCommandValidator(IUnitOfWork uow)
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
                .MustAsync(TrabajadorSeguroDoesNotExist)
                .WithMessage("El trabajador ya tiene otro registro de seguro.");

            RuleFor(x => x.EpsCode)
                .MustAsync(EpsExists)
                .WithMessage("La EPS indicada no existe.");

            RuleFor(x => x.SituacionEpsCode)
                .MustAsync(SituacionEpsExists)
                .WithMessage("La situación EPS indicada no existe.");

            RuleFor(x => x.SctrTipoCode)
                .MustAsync(TipoSctrExists)
                .WithMessage("El tipo de SCTR indicado no existe.");

            RuleFor(x => x.SctrSaludCode)
                .MustAsync(SctrSaludExists)
                .WithMessage("El SCTR Salud indicado no existe.");

            RuleFor(x => x.SctrPensionCode)
                .MustAsync(SctrPensionExists)
                .WithMessage("El SCTR Pensión indicado no existe.");

            RuleFor(x => x.NumeroSeguro)
                .MaximumLength(50)
                .WithMessage("El número de seguro no puede superar los 50 caracteres.");

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

        private async Task<bool> TrabajadorSeguroDoesNotExist(
            UpdateTrabajadorSeguroCommand command,
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return !await _uow.Rrhh.Maestros.TrabajadorSeguros
                .Query()
                .AnyAsync(
                    x => x.TrabajadorCode == trabajadorCode.Trim()
                         && x.Id != command.Id,
                    cancellationToken);
        }

        private async Task<bool> EpsExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.Epss
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> SituacionEpsExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.SituacionesEps
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> TipoSctrExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.TiposSctr
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> SctrSaludExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.SctrsSalud
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> SctrPensionExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.SctrsPension
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }
    }
}