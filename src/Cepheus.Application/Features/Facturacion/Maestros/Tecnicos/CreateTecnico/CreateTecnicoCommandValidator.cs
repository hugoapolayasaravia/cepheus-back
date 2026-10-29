using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.CreateTecnico
{
    public class CreateTecnicoCommandValidator : AbstractValidator<CreateTecnicoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTecnicoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código del trabajador es obligatorio.")
                .Length(5).WithMessage("El código del trabajador debe tener 5 caracteres.")
                .MustAsync(TrabajadorExiste).WithMessage("El trabajador indicado no existe en RRHH.")
                .MustAsync(NoEsYaTecnico).WithMessage("El trabajador ya está habilitado como técnico.");
        }

        private async Task<bool> TrabajadorExiste(string trabajadorCode, CancellationToken cancellationToken)
            => await _uow.Rrhh.Maestros.Trabajadores.Query()
                .AnyAsync(t => t.Code == trabajadorCode, cancellationToken);

        private async Task<bool> NoEsYaTecnico(string trabajadorCode, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Maestros.Tecnicos.Query()
                .AnyAsync(t => t.TrabajadorCode == trabajadorCode, cancellationToken);
    }
}
