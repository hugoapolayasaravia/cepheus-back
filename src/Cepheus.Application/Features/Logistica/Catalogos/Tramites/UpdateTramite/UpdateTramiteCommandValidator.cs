using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.UpdateFamilia;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Tramites.UpdateTramite
{
    public class UpdateTramiteCommandValidator : AbstractValidator<UpdateTramiteCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTramiteCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;
    
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del trámite es obligatorio.")
                .Length(1).WithMessage("El código debe tener 1 carácter.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del trámite es obligatorio.")
                .MaximumLength(20).WithMessage("El nombre no puede exceder los 20 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra Tramite con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateTramiteCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.Tramites.Query()
                .AnyAsync(f => f.Code != command.Code && f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
