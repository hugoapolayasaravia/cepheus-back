using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.UpdateUnidadMedida
{
    public class UpdateUnidadMedidaCommandValidator
        : AbstractValidator<UpdateUnidadMedidaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateUnidadMedidaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El código de la unidad de medida es obligatorio.")
                .Length(2)
                .WithMessage("El código debe tener exactamente 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El nombre de la unidad de medida es obligatorio.")
                .MaximumLength(50)
                .WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName)
                .WithMessage("Ya existe otra unidad de medida con ese nombre.");

            RuleFor(x => x.IndDecimal)
               .Cascade(CascadeMode.Stop)
               .NotEmpty()
               .WithMessage("Debe indicar si la unidad de medida tiene decimales S / N.");


            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(
            UpdateUnidadMedidaCommand command,
            string name,
            CancellationToken cancellationToken)
        {
            var normalizedName = name.Trim().ToLower();

            return !await _uow.Logistica.Catalogos.UnidadesMedida
                .Query()
                .AnyAsync(
                    u => u.Code != command.Code &&
                         u.Name.ToLower() == normalizedName,
                    cancellationToken);
        }
    }
}