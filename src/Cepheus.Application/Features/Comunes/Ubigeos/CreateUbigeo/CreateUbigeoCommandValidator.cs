using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Ubigeos.CreateUbigeo
{
    public class CreateUbigeoCommandValidator : AbstractValidator<CreateUbigeoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateUbigeoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código de ubigeo es obligatorio.")
                .Length(6).WithMessage("El código de ubigeo debe tener 6 dígitos (2 departamento + 2 provincia + 2 distrito).")
                .Matches("^[0-9]{6}$").WithMessage("El código de ubigeo debe contener solo dígitos.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un ubigeo con ese código.");

            RuleFor(x => x.Department)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El departamento es obligatorio.")
                .MaximumLength(50).WithMessage("El departamento no puede exceder los 50 caracteres.");

            RuleFor(x => x.Province)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La provincia es obligatoria.")
                .MaximumLength(50).WithMessage("La provincia no puede exceder los 50 caracteres.");

            RuleFor(x => x.District)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El distrito es obligatorio.")
                .MaximumLength(50).WithMessage("El distrito no puede exceder los 50 caracteres.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
            => !await _uow.Comunes.Ubigeos.Query()
                .AnyAsync(u => u.Code == code.Trim(), cancellationToken);
    }
}
