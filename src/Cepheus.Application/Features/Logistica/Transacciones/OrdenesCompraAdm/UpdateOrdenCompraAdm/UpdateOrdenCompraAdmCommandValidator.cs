// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/UpdateOrdenCompraAdministracion/UpdateOrdenCompraAdministracionCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.UpdateOrdenCompraAdm
{
    public class UpdateOrdenCompraAdmCommandValidator
        : OrdenCompraAdmCabeceraValidator<UpdateOrdenCompraAdmCommand>
    {
        public UpdateOrdenCompraAdmCommandValidator(IUnitOfWork uow) : base(uow)
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(6);
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }
}
