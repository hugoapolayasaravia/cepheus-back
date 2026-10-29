using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.CreateTrabajadorRemuneracion
{
    public class CreateTrabajadorRemuneracionCommandValidator
        : AbstractValidator<CreateTrabajadorRemuneracionCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorRemuneracionCommandValidator(IUnitOfWork uow)
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
                .MustAsync(BeUniqueTrabajador)
                .WithMessage("El trabajador ya tiene una remuneración registrada.");

            RuleFor(x => x.SueldoBasico)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El sueldo básico no puede ser negativo.");

            RuleFor(x => x.MonedaCode)
                .MustAsync(MonedaExists)
                .WithMessage("La moneda indicada no existe.");

            RuleFor(x => x.ModoPagoCode)
                .MustAsync(ModoPagoExists)
                .WithMessage("El modo de pago indicado no existe.");
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
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return !await _uow.Rrhh.Maestros.TrabajadorRemuneracions
                .Query()
                .AnyAsync(
                    x => x.TrabajadorCode == trabajadorCode.Trim(),
                    cancellationToken);
        }

        private async Task<bool> MonedaExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Comunes.Monedas
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> ModoPagoExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.ModosPago
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }
    }
}
