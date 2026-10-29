using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.CreateTrabajadorSindicato
{
    public class CreateTrabajadorSindicatoCommandValidator
        : AbstractValidator<CreateTrabajadorSindicatoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorSindicatoCommandValidator(IUnitOfWork uow)
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
                .MustAsync(TrabajadorSindicatoDoesNotExist)
                .WithMessage("El trabajador ya tiene un registro de afiliación sindical.");
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
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return !await _uow.Rrhh.Maestros.TrabajadorSindicatos
                .Query()
                .AnyAsync(
                    x => x.TrabajadorCode == trabajadorCode.Trim(),
                    cancellationToken);
        }
    }
}
