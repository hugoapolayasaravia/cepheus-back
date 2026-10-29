using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.CreateUnidadNegocio
{
    public class CreateUnidadNegocioCommandValidator : AbstractValidator<CreateUnidadNegocioCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateUnidadNegocioCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("Ya existe una unidad de negocio con ese nombre.");

            RuleFor(x => x.ParentCode)
                .MustAsync(ParentExists).WithMessage("La unidad de negocio padre indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ParentCode));
        }

        private async Task<bool> BeUniqueName(string? name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.UnidadesNegocio.Query()
                .AnyAsync(u => u.Name != null && u.Name.ToLower() == name!.Trim().ToLower(), cancellationToken);

        private async Task<bool> ParentExists(string? parentCode, CancellationToken cancellationToken)
            => await _uow.Logistica.Catalogos.UnidadesNegocio.Query()
                .AnyAsync(u => u.Code == parentCode!.Trim().ToUpper(), cancellationToken);
    }
}