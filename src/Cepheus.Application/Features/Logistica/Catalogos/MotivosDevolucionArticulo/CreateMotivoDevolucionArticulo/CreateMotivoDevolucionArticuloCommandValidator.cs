using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.CreateMotivoDevolucionArticulo;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucion.CreateMotivoDevolucionArticulo
{
    public class CreateMotivoDevolucionArticuloCommandValidator : AbstractValidator<CreateMotivoDevolucionArticuloCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateMotivoDevolucionArticuloCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

        
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del motivo de devolución es obligatorio.")
                .MaximumLength(80).WithMessage("El nombre no puede exceder los 80 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un motivo de devolucion con ese nombre."); ;
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Comunes.MotivosDevolucion.Query()
                .AnyAsync(m => m.Name == name.Trim().ToUpper(), cancellationToken);
    }
}
