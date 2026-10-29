using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.CreateTipoHojaCorte
{
    public class CreateTipoHojaCorteCommandValidator : AbstractValidator<CreateTipoHojaCorteCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoHojaCorteCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de hoja de corte es obligatorio.")
                .MaximumLength(30).WithMessage("El nombre no puede exceder los 30 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un tipo de hoja de corte con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.TiposHojaCorte.Query()
                .AnyAsync(t => t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
