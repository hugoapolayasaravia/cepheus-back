using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.UpdateFamilia;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Compradores.UpdateComprador
{
    public class UpdateCompradorCommandValidator : AbstractValidator<UpdateCompradorCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCompradorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;
    
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del comprador es obligatorio.")
                .Length(3).WithMessage("El código debe tener 3 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del comprador es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra comprador con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateCompradorCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.Compradores.Query()
                .AnyAsync(f => f.Code != command.Code && f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}