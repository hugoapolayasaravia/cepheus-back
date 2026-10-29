using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.UpdateTrabajadorSindicato
{
    public class UpdateTrabajadorSindicatoCommandValidator
        : AbstractValidator<UpdateTrabajadorSindicatoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorSindicatoCommandValidator(IUnitOfWork uow)
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
                .MustAsync(TrabajadorSindicatoDoesNotExist)
                .WithMessage("El trabajador ya tiene otro registro de afiliación sindical.");

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

        private async Task<bool> TrabajadorSindicatoDoesNotExist(
            UpdateTrabajadorSindicatoCommand command,
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return !await _uow.Rrhh.Maestros.TrabajadorSindicatos
                .Query()
                .AnyAsync(
                    x => x.TrabajadorCode == trabajadorCode.Trim()
                         && x.Id != command.Id,
                    cancellationToken);
        }
    }
}
