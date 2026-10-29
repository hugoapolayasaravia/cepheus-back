// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion.OrdenCompraAdministracionDetalles/Common/OrdenCompraAdministracionLineaValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.Common
{
    public abstract class OrdenCompraAdmLineaValidator<T> : AbstractValidator<T>
        where T : IOrdenCompraAdmLinea
    {
        protected OrdenCompraAdmLineaValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.Glosa)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.UnidadMedidaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Catalogos.UnidadesMedida.Query()
                        .AnyAsync(u => u.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La unidad de medida indicada no existe.");

            RuleFor(x => x.Cantidad)
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a 0.");

            RuleFor(x => x.Precio)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El precio no puede ser negativo.");

            RuleFor(x => x.Descuento)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El descuento no puede ser negativo.");

            RuleFor(x => x.SubCentroCostoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Maestros.SubCentrosCosto.Query()
                        .AnyAsync(s => s.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El subcentro de costo indicado no existe.");
        }
    }
}
