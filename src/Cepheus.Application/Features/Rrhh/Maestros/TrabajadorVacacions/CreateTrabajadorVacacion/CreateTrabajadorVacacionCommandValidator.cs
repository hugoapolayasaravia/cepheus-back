using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.CreateTrabajadorVacacion
{
    public class CreateTrabajadorVacacionCommandValidator
        : AbstractValidator<CreateTrabajadorVacacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorVacacionCommandValidator(IUnitOfWork uow)
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

            RuleFor(x => x.FechaVacaciones)
                .Must(fecha =>
                    !fecha.HasValue ||
                    fecha.Value.Date >= DateTime.UtcNow.Date)
                .WithMessage("La fecha de vacaciones no puede ser anterior a la fecha actual.");
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
    }
}
