// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion.OrdenCompraAdministracionDetalles/UpdateOrdenCompraAdministracionDetalle/UpdateOrdenCompraAdministracionDetalleCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.UpdateOrdenCompraAdmDetalle
{
    public class UpdateOrdenCompraAdmDetalleCommandValidator
        : OrdenCompraAdmLineaValidator<UpdateOrdenCompraAdmDetalleCommand>
    {
        public UpdateOrdenCompraAdmDetalleCommandValidator(IUnitOfWork uow) : base(uow)
        {
            RuleFor(x => x.OrdenCompraAdministracionCode).NotEmpty().MaximumLength(6);
            RuleFor(x => x.ItemNumber).GreaterThan(0);
        }
    }
}
