using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.CreateSubCentroEjecutor
{
    public class CreateSubCentroEjecutorCommandValidator : AbstractValidator<CreateSubCentroEjecutorCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateSubCentroEjecutorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del subcentro ejecutor es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un subcentro ejecutor con ese nombre.");

            RuleFor(x => x.CentroEjecutorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El centro ejecutor es obligatorio.")
                .MustAsync(CentroEjecutorExists).WithMessage("El centro ejecutor indicado no existe.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Maestros.SubCentrosEjecutores.Query()
                .AnyAsync(s => s.Name.ToLower() == name.Trim().ToLower(), cancellationToken);

        private async Task<bool> CentroEjecutorExists(string code, CancellationToken cancellationToken)
            => await _uow.Mantenimiento.Maestros.CentrosEjecutores.Query()
                .AnyAsync(c => c.Code == code.Trim().ToUpper(), cancellationToken);
    }
}
