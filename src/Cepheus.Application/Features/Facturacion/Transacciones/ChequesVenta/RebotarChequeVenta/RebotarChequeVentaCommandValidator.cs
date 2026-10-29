using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.RebotarChequeVenta
{
    public class RebotarChequeVentaCommandValidator : AbstractValidator<RebotarChequeVentaCommand>
    {
        private readonly IUnitOfWork _uow;

        public RebotarChequeVentaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.ClienteCode).NotEmpty().MaximumLength(5);
            RuleFor(x => x.BancoCode).NotEmpty().MaximumLength(3);
            RuleFor(x => x.Number).NotEmpty().MaximumLength(15);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("La versión de la fila es obligatoria.");

            RuleFor(x => x.MotivoReboteCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Debe indicar el motivo del rebote.")
                .Length(2).WithMessage("El código del motivo debe tener 2 caracteres.")
                .MustAsync(MotivoExistsAndActive).WithMessage("El motivo de rebote no existe o está inactivo.");

            RuleFor(x => x.ReboteDate)
                .NotEmpty().WithMessage("Debe indicar la fecha del rebote.");
        }

        private async Task<bool> MotivoExistsAndActive(string code, CancellationToken cancellationToken)
        {
            var c = code.Trim();
            return await _uow.Facturacion.Catalogos.MotivosChequeRebote.Query()
                .AnyAsync(m => m.Code == c && m.IsActive, cancellationToken);
        }
    }
}
