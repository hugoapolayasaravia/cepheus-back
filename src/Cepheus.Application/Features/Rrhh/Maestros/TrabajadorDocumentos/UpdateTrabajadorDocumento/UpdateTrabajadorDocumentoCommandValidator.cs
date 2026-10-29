using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.UpdateTrabajadorDocumento
{
    public class UpdateTrabajadorDocumentoCommandValidator : AbstractValidator<UpdateTrabajadorDocumentoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorDocumentoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TipoDocumentoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de documento es obligatorio.")
                .MustAsync(TipoDocumentoExists).WithMessage("El tipo de documento indicado no existe.");

            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("El número de documento es obligatorio.")
                .MaximumLength(50).WithMessage("El número de documento no puede exceder los 50 caracteres.");

            RuleFor(x => x)
                .MustAsync(BeUniqueDocument)
                .WithMessage("Ya existe otro documento con ese tipo y número.")
                .WithName("DocumentNumber");
        }

        private async Task<bool> TipoDocumentoExists(string tipoDocumentoCode, CancellationToken cancellationToken)
            => await _uow.Comunes.TiposDocumento.Query()
                .AnyAsync(t => t.Code == tipoDocumentoCode.Trim().ToUpper(), cancellationToken);

        private async Task<bool> BeUniqueDocument(UpdateTrabajadorDocumentoCommand command, CancellationToken cancellationToken)
            => !await _uow.Rrhh.Maestros.TrabajadorDocumentos.Query()
                .AnyAsync(d =>
                    d.Id != command.Id &&
                    d.TipoDocumentoCode == command.TipoDocumentoCode.Trim().ToUpper() &&
                    d.DocumentNumber == command.DocumentNumber.Trim(),
                    cancellationToken);
    }
}