using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.CreateAnexo
{
    public class CreateAnexoCommandValidator : AbstractValidator<CreateAnexoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateAnexoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TipoAnexoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de anexo es obligatorio.")
                .Length(3).WithMessage("El tipo de anexo debe tener 3 caracteres.")
                .MustAsync(TipoAnexoExists).WithMessage("El tipo de anexo indicado no existe.");

            RuleFor(x => x.Name)
                .MaximumLength(150).WithMessage("La descripción no puede exceder los 150 caracteres.");

            RuleFor(x => x.LinkAccount1Code)
                .MaximumLength(8).WithMessage("La cuenta de enlace 1 no puede exceder los 8 caracteres.");

            RuleFor(x => x.LinkAccount2Code)
                .MaximumLength(8).WithMessage("La cuenta de enlace 2 no puede exceder los 8 caracteres.");

            RuleFor(x => x.Flag)
                .MaximumLength(1).WithMessage("El flag no puede exceder 1 carácter.");
        }

        private async Task<bool> TipoAnexoExists(string code, CancellationToken ct)
            => await _uow.Contabilidad.Catalogos.TiposAnexo.Query().AnyAsync(t => t.Code == code.Trim(), ct);
    }
}
