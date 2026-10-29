using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.UpdateSubCentroEjecutor
{
    public class UpdateSubCentroEjecutorCommandValidator : AbstractValidator<UpdateSubCentroEjecutorCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSubCentroEjecutorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del subcentro ejecutor es obligatorio.")
                .MaximumLength(4).WithMessage("El código no puede exceder los 4 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del subcentro ejecutor es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro subcentro ejecutor con ese nombre.");

            RuleFor(x => x.CentroEjecutorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El centro ejecutor es obligatorio.")
                .MustAsync(CentroEjecutorExists).WithMessage("El centro ejecutor indicado no existe.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateSubCentroEjecutorCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Maestros.SubCentrosEjecutores.Query()
                .AnyAsync(s => s.Code != command.Code && s.Name.ToLower() == name.Trim().ToLower(), cancellationToken);

        private async Task<bool> CentroEjecutorExists(string code, CancellationToken cancellationToken)
            => await _uow.Mantenimiento.Maestros.CentrosEjecutores.Query()
                .AnyAsync(c => c.Code == code.Trim().ToUpper(), cancellationToken);
    }
}
