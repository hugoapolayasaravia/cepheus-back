using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.CreateTrabajadorContrato
{
    public class CreateTrabajadorContratoCommandValidator
        : AbstractValidator<CreateTrabajadorContratoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorContratoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El trabajador es obligatorio.")
                .Length(5)
                .WithMessage("El código de trabajador debe tener 5 caracteres.")
                .MustAsync(TrabajadorExists)
                .WithMessage("El trabajador indicado no existe.");

            RuleFor(x => x.TipoContratoCode)
                .MustAsync(TipoContratoExists)
                .WithMessage("El tipo de contrato indicado no existe.");

            RuleFor(x => x.TipoExtensionCode)
                .MustAsync(TipoExtensionContratoExists)
                .WithMessage("El tipo de extensión indicado no existe.");

            RuleFor(x => x)
                .Must(x =>
                    !x.FechaInicio.HasValue ||
                    !x.FechaFin.HasValue ||
                    x.FechaFin.Value >= x.FechaInicio.Value)
                .WithMessage("La fecha fin no puede ser anterior a la fecha de inicio.");

            RuleFor(x => x)
                .Must(x =>
                    !x.FechaInicio.HasValue ||
                    !x.FechaTermino.HasValue ||
                    x.FechaTermino.Value >= x.FechaInicio.Value)
                .WithMessage("La fecha de término no puede ser anterior a la fecha de inicio.");

            RuleFor(x => x.CantidadDuracion)
                .GreaterThan(0)
                .When(x => x.CantidadDuracion.HasValue)
                .WithMessage("La cantidad de duración debe ser mayor que cero.");

            RuleFor(x => x)
                .Must(BeValidDuration)
                .WithMessage("Debe indicar la cantidad de duración cuando se especifica el tipo de duración.");
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

        private async Task<bool> TipoContratoExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.TiposContrato
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> TipoExtensionContratoExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.TiposExtensionContrato
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private static bool BeValidDuration(
            CreateTrabajadorContratoCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.TipoDuracion))
                return true;

            return command.CantidadDuracion.HasValue &&
                   command.CantidadDuracion.Value > 0;
        }
    }
}
