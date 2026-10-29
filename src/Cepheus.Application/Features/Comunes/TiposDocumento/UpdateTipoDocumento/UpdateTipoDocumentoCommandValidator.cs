using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.ComprobantesPago.UpdateComprobantePago;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.TiposDocumento.UpdateTipoDocumento
{
    public class UpdateTipoDocumentoCommandValidator : AbstractValidator<UpdateTipoDocumentoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoDocumentoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código del tipo de documento es obligatorio.")
                .MaximumLength(3)
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un tipo de documento con ese código.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de documento es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un tipo de documento con ese nombre.");

            RuleFor(x => x.ShortName)
                .MaximumLength(3);

            RuleFor(x => x.SunatCode)
                .MaximumLength(2);
        }

        private async Task<bool> BeUniqueCode(UpdateTipoDocumentoCommand command, string code, CancellationToken cancellationToken)
            => !await _uow.Comunes.TiposDocumento.Query()
                .AnyAsync(t => t.Code == code.Trim().ToUpper() && t.Code != command.Code, cancellationToken);

        private async Task<bool> BeUniqueName(UpdateTipoDocumentoCommand command, string name, CancellationToken cancellationToken)
                  => !await _uow.Comunes.TiposDocumento.Query()
                      .AnyAsync(b => b.Code != command.Code && b.Name.ToLower() == name.Trim().ToLower(), cancellationToken);


    }
}
