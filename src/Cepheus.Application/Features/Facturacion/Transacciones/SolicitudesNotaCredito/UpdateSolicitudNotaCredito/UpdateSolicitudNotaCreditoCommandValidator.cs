using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.UpdateSolicitudNotaCredito
{
    public class UpdateSolicitudNotaCreditoCommandValidator
        : SolicitudNotaCreditoInputValidator<UpdateSolicitudNotaCreditoCommand>
    {
        public UpdateSolicitudNotaCreditoCommandValidator(IUnitOfWork uow) : base(uow)
        {
            RuleFor(x => x.Year).NotEmpty().Length(4).WithMessage("El año debe tener 4 caracteres.");
            RuleFor(x => x.Month).NotEmpty().Length(2).WithMessage("El mes debe tener 2 caracteres.");
            RuleFor(x => x.Code).NotEmpty().WithMessage("El código de la solicitud es obligatorio.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
