using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Bancos.UpdateBanco;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.ComprobantesPago.UpdateComprobantePago
{
    public class UpdateComprobantePagoCommandValidator : AbstractValidator<UpdateComprobantePagoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateComprobantePagoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código del comprobante es obligatorio.")
                .MaximumLength(2)
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un comprobante con ese código.");

            RuleFor(x => x.SunatCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código SUNAT es obligatorio.")
                .MaximumLength(2)
                .MustAsync(BeUniqueSunatCode).WithMessage("Ya existe un comprobante con ese código SUNAT.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del comprobante es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un comprobante con ese nombre.");

            RuleFor(x => x.ShortName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La abreviatura del comprobante es obligatoria.")
                .MaximumLength(10);

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("El nombre no puede exceder los 255 caracteres.");
        }

        private async Task<bool> BeUniqueCode(UpdateComprobantePagoCommand command, string code, CancellationToken cancellationToken)
            => !await _uow.Comunes.ComprobantesPago.Query()
                .AnyAsync(c => c.Code == code.Trim().ToUpper() && c.Code != command.Code, cancellationToken);

        private async Task<bool> BeUniqueSunatCode(UpdateComprobantePagoCommand command, string sunatCode, CancellationToken cancellationToken)
            => !await _uow.Comunes.ComprobantesPago.Query()
                .AnyAsync(c => c.SunatCode == sunatCode.Trim() && c.Code != command.Code, cancellationToken);
        private async Task<bool> BeUniqueName(UpdateComprobantePagoCommand command, string name, CancellationToken cancellationToken)
                 => !await _uow.Comunes.ComprobantesPago.Query()
                     .AnyAsync(b => b.Code != command.Code && b.Name.ToLower() == name.Trim().ToLower(), cancellationToken);

    }
}
