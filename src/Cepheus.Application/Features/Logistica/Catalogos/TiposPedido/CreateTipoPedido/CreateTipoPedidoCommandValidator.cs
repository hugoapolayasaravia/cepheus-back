using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.CreateTipoPedido
{
    public class CreateTipoPedidoCommandValidator : AbstractValidator<CreateTipoPedidoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoPedidoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de pedido es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una familia con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.TiposPedido.Query()
                .AnyAsync(f => f.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}