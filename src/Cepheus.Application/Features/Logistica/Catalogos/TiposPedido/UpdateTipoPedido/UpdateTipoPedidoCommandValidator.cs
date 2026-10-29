using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.UpdateFamilia;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.UpdateTipoPedido
{
    public class UpdateTipoPedidoCommandValidator : AbstractValidator<UpdateTipoPedidoCommand>
    {
        private readonly IUnitOfWork _uow;
        public UpdateTipoPedidoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow; 
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del tipo de pedido es obligatorio.")
                .Length(2).WithMessage("El código debe tener 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de pedido es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra familia con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateTipoPedidoCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.TiposPedido.Query()
                .AnyAsync(f => f.Code != command.Code && f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}