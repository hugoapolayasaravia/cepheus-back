using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Ubigeos.UpdateUbigeo
{
    public class UpdateUbigeoCommandValidator : AbstractValidator<UpdateUbigeoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateUbigeoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código de ubigeo es obligatorio.")
                .Length(6).WithMessage("El código de ubigeo debe tener 6 dígitos.")
                .Matches("^[0-9]{6}$").WithMessage("El código de ubigeo debe contener solo dígitos.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un ubigeo con ese código.");

            RuleFor(x => x.Department)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El departamento es obligatorio.")
                .MaximumLength(50);

            RuleFor(x => x.Province)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La provincia es obligatoria.")
                .MaximumLength(50);

            RuleFor(x => x.District)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El distrito es obligatorio.")
                .MaximumLength(50);
        }

        private async Task<bool> BeUniqueCode(UpdateUbigeoCommand command, string code, CancellationToken cancellationToken)
            => !await _uow.Comunes.Ubigeos.Query()
                .AnyAsync(u => u.Code == code.Trim() && u.Code != command.Code, cancellationToken);
    }
}
