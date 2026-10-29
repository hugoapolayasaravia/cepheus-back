// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion.OrdenCompraAdministracionDetalles/CreateOrdenCompraAdministracionDetalle/CreateOrdenCompraAdministracionDetalleCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.CreateOrdenCompraAdmDetalle
{
    public class CreateOrdenCompraAdmDetalleCommandValidator
        : OrdenCompraAdmLineaValidator<CreateOrdenCompraAdmDetalleCommand>
    {
        public CreateOrdenCompraAdmDetalleCommandValidator(IUnitOfWork uow) : base(uow)
        {
            RuleFor(x => x.OrdenCompraAdministracionCode).NotEmpty().MaximumLength(6);
        }
    }
}
