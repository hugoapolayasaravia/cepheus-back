using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.ComprobantesPago.CreateComprobantePago
{
    public class CreateComprobantePagoCommandValidator : AbstractValidator<CreateComprobantePagoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateComprobantePagoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;


            RuleFor(x => x.SunatCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código SUNAT es obligatorio.")
                .MaximumLength(2).WithMessage("El código SUNAT no puede exceder los 2 caracteres.")
                .MustAsync(BeUniqueSunatCode).WithMessage("Ya existe un comprobante con ese código SUNAT.");
                 

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del comprobante es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un comprobante con ese nombre."); 

            RuleFor(x => x.ShortName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La abreviatura del comprobante es obligatoria.")
                .MaximumLength(10).WithMessage("La abreviatura no puede exceder los 10 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("La descripción no puede exceder los 255 caracteres.");
        }


        private async Task<bool> BeUniqueSunatCode(string sunatCode, CancellationToken cancellationToken)
            => !await _uow.Comunes.ComprobantesPago.Query()
                .AnyAsync(c => c.SunatCode == sunatCode.Trim(), cancellationToken);

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Comunes.Bancos.Query()
                .AnyAsync(b => b.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
