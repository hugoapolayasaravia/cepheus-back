using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.CreateChofer
{
    public class CreateChoferCommandValidator : AbstractValidator<CreateChoferCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateChoferCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TransportistaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El transportista es obligatorio.")
                .MustAsync(TransportistaExists).WithMessage("El transportista indicado no existe.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("El nombre del chofer es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.DriverLicenseNumber)
                .NotEmpty().WithMessage("El número de brevete es obligatorio.")
                .MaximumLength(20).WithMessage("El brevete no puede exceder los 20 caracteres.");

            RuleFor(x => x.Observations).MaximumLength(500).WithMessage("Las observaciones no pueden exceder los 500 caracteres.");
        }

        private async Task<bool> TransportistaExists(string code, CancellationToken ct)
            => await _uow.Facturacion.Maestros.Transportistas.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct);
    }
}
