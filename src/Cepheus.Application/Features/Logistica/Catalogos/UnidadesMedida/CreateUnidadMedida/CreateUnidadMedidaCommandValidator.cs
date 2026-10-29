using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.CreateUnidadMedida
{
    public class CreateUnidadMedidaCommandValidator
        : AbstractValidator<CreateUnidadMedidaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateUnidadMedidaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El código de la unidad de medida es obligatorio.")
                .MaximumLength(2)
                .WithMessage("El código de la unidad de medida no puede exceder los 2 caracteres.")
                .MustAsync(BeUniqueCode)
                .WithMessage("Ya existe una unidad de medida con ese código.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El nombre de la unidad de medida es obligatorio.")
                .MaximumLength(50)
                .WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName)
                .WithMessage("Ya existe una unidad de medida con ese nombre.");

            RuleFor(x => x.IndDecimal)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Debe indicar si la unidad de medida tiene decimales S / N.");
        }

        private async Task<bool> BeUniqueCode(
            string code,
            CancellationToken cancellationToken)
        {
            var normalizedCode = code.Trim().ToUpper();

            return !await _uow.Logistica.Catalogos.UnidadesMedida.Query()
                .AnyAsync(
                    u => u.Code.ToUpper() == normalizedCode,
                    cancellationToken);
        }

        private async Task<bool> BeUniqueName(
            string name,
            CancellationToken cancellationToken)
        {
            var normalizedName = name.Trim().ToLower();

            return !await _uow.Logistica.Catalogos.UnidadesMedida.Query()
                .AnyAsync(
                    u => u.Name.ToLower() == normalizedName,
                    cancellationToken);
        }
    }
}