using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.UpdateTrabajadorJornada
{
    public class UpdateTrabajadorJornadaCommandValidator
        : AbstractValidator<UpdateTrabajadorJornadaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorJornadaCommandValidator(IUnitOfWork uow)
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
                .MustAsync(TrabajadorJornadaDoesNotExist)
                .WithMessage("El trabajador ya tiene otro registro de jornada.");

            RuleFor(x => x.HorarioCode)
                .MustAsync(HorarioExists)
                .WithMessage("El horario indicado no existe.");

            RuleFor(x => x.HorasExtCon125)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Las horas extras con 125% no pueden ser negativas.");

            RuleFor(x => x.HorasExtCon135)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Las horas extras con 135% no pueden ser negativas.");

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

        private async Task<bool> TrabajadorJornadaDoesNotExist(
            UpdateTrabajadorJornadaCommand command,
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return !await _uow.Rrhh.Maestros.TrabajadorJornadas
                .Query()
                .AnyAsync(
                    x => x.TrabajadorCode == trabajadorCode.Trim()
                             && x.Id != command.Id,
                    cancellationToken);
        }

        private async Task<bool> HorarioExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.Horarios
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }
    }
}
