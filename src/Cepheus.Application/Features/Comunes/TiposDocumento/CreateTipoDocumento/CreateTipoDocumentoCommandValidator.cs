using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.TiposDocumento.CreateTipoDocumento
{
    public class CreateTipoDocumentoCommandValidator : AbstractValidator<CreateTipoDocumentoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoDocumentoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código del tipo de documento es obligatorio.")
                .MaximumLength(2).WithMessage("El código no puede exceder los 2 caracteres.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un tipo de documento con ese código.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de documento es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un tipo de documento con ese nombre.");

            RuleFor(x => x.ShortName)
                .MaximumLength(3).WithMessage("La abreviatura no puede exceder los 3 caracteres.");

            RuleFor(x => x.SunatCode)
                .MaximumLength(2).WithMessage("El código SUNAT no puede exceder los 2 caracteres.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
            => !await _uow.Comunes.TiposDocumento.Query()
                .AnyAsync(t => t.Code == code.Trim().ToUpper(), cancellationToken);

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        => !await _uow.Comunes.TiposDocumento.Query()
            .AnyAsync(b => b.Name.ToLower() == name.Trim().ToLower(), cancellationToken);

    }
}
