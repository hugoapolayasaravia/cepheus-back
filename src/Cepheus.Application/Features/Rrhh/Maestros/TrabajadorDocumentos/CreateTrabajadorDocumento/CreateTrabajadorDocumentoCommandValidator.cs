using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.CreateTrabajadorDocumento
{
    public class CreateTrabajadorDocumentoCommandValidator : AbstractValidator<CreateTrabajadorDocumentoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorDocumentoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El trabajador es obligatorio.")
                .Length(5).WithMessage("El código de trabajador debe tener 5 caracteres.")
                .MustAsync(TrabajadorExists).WithMessage("El trabajador indicado no existe.");

            RuleFor(x => x.TipoDocumentoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de documento es obligatorio.")
                .MustAsync(TipoDocumentoExists).WithMessage("El tipo de documento indicado no existe.");

            RuleFor(x => x.DocumentNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El número de documento es obligatorio.")
                .MaximumLength(50).WithMessage("El número de documento no puede exceder los 50 caracteres.");

            RuleFor(x => x)
                .MustAsync(BeUniqueDocument)
                .WithMessage("Ya existe un documento con ese tipo y número.")
                .WithName("DocumentNumber");
        }

        private async Task<bool> TrabajadorExists(string trabajadorCode, CancellationToken cancellationToken)
            => await _uow.Rrhh.Maestros.Trabajadores.Query()
                .AnyAsync(t => t.Code == trabajadorCode.Trim().ToUpper(), cancellationToken);

        private async Task<bool> TipoDocumentoExists(string tipoDocumentoCode, CancellationToken cancellationToken)
            => await _uow.Comunes.TiposDocumento.Query()
                .AnyAsync(t => t.Code == tipoDocumentoCode.Trim().ToUpper(), cancellationToken);

        private async Task<bool> BeUniqueDocument(CreateTrabajadorDocumentoCommand command, CancellationToken cancellationToken)
            => !await _uow.Rrhh.Maestros.TrabajadorDocumentos.Query()
                .AnyAsync(d =>
                    d.TipoDocumentoCode == command.TipoDocumentoCode.Trim().ToUpper() &&
                    d.DocumentNumber == command.DocumentNumber.Trim(),
                    cancellationToken);
    }
}